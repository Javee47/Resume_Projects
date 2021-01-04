// The MIT License (MIT)
// 
// Copyright (c) 2016, 2017 Trevor Bakker 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#define _GNU_SOURCE

#include <stdio.h>
#include <stdint.h>
#include <unistd.h>
#include <sys/wait.h>
#include <stdlib.h>
#include <errno.h>
#include <string.h>
#include <signal.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <time.h>
#define WHITESPACE " \t\n"      // We want to split our command line up into tokens
                                // so we need to define what delimits our tokens.
                                // In this case  white space
                                // will separate the tokens on our command line

#define MAX_COMMAND_SIZE 255    // The maximum command-line size

#define MAX_NUM_ARGUMENTS 5     // Mav shell only supports five arguments
#define NUM_BLOCKS 4226
#define BLOCK_SIZE 8192
#define NUM_FILES 128
#define MAX_FILE_SIZE 10240000
FILE *fd;
uint8_t blocks[NUM_BLOCKS][BLOCK_SIZE];

// file system data structures from class
struct Directory_Entry
{
	uint8_t valid;
	char filename[255];
	uint32_t inode;
};
struct Inode
{
	uint8_t valid;
	uint8_t attributes;
	uint32_t size;
	uint32_t blocks[1250];
};

//Globals from class
struct Directory_Entry *dir;
struct Inode *inodes;
uint8_t * freeBlockList;
uint8_t * freeInodeList; 

//current fs is a string with the name of the current file system
char* currentfs;

// first few functions taken from class and examples 
void initializeDirectory()
{
	int i;
	for (i=0;i<NUM_FILES; i++)
	{
		dir[i].valid = 0;
		dir[i].inode = -1;
		memset(dir[i].filename,0,255);
	}
}
void initializeBlockList()
{
	int i;
	for (i=0;i<NUM_BLOCKS;i++)
	{
		freeBlockList[i]=0;
		if (i < 10)
		{
			freeBlockList[i] = 1;
		}
	}
}
void initializeInodes()
{
	int i;
	for(i=0;i<NUM_FILES;i++)
	{
		inodes[i].valid = 0;
		inodes[i].size = 0;
		inodes[i].attributes = 0;
		//I removed this section because it caused alot of over writing bugs
		//C on omega automatically sets the inode.blocks array to 0 anyway 
		//This technically points to the first data block which could cause bugs normally
		//since things like del would overwrite directory with 0's
		//but i've programmed my put, del, and get loops
		//to not mess with any of the data from the blocks structure with indexes less than 10
		//unless we are loading in a new file system
		//so it should work fine

		//int j;
		//printf("%d ", inodes[i].blocks[0]);
		/*for (j=0; j<1250; j++)
		{
			if (j>9)
			{
			//	inodes[i].blocks[j] = 0;
			}	
		}*/
	}
}
void initializeInodeList()
{
	int i;
	for (i=0;i<NUM_FILES;i++)
	{
		freeInodeList[i] =0;
	}
}

//useful functions made in class
int df()
{
	int i;
	int free_space = 0;
	for (i=0;i<NUM_BLOCKS;i++)
	{
		if(freeBlockList[i]==0)
		{
			free_space = free_space +BLOCK_SIZE;
		}
	}
	return free_space;
}
int findDirectoryEntry(char* filename)
{
	int i;
	for(i=0;i<NUM_FILES;i++)
	{
		if(strcmp(filename,dir[i].filename)==0)
		{
			return i;
		}
	
	}
	for (i=0;i<NUM_FILES;i++)
	{
		if (dir[i].valid==0)
		{
			dir[i].valid = 1;
			freeBlockList[i] =1;
			return i;
		}
	}
	return -1;
}
int findFreeBlock()
{
	int i;
	int ret=-1;
	for (i=0;i<NUM_BLOCKS;i++)
	{
		if (freeBlockList[i]==0)
		{
			freeBlockList[i]=1;
			return i;
		}
	}
	return ret;
}
int findFreeInode()
{
	int i;
	int ret =-1;
	for (i=0;i<NUM_FILES;i++)
	{
		if (inodes[i].valid == 0)
		{
			inodes[i].valid = 1;
			return i;
		}
	}
	return ret;
}
// accessible by user functions
// put function from class with a few addons
void put(char * filename)
{
	int ret;
	struct stat buf;
	ret = stat(filename,&buf);
	if (ret==-1)
	{
		printf("File does not exist\n");
		return -1;
	}
	int size = buf.st_size;
	
	if (size > MAX_FILE_SIZE)
	{
		printf("File size too big\n");
		return -1;
	}
	if (size > df())
	{
		printf("File exceeds remaining disk space\n");
		return -1;
	}
	
	int directoryIndex = findDirectoryEntry(filename);
	int inodeIndex = findFreeInode();
        dir[directoryIndex].valid = 1;
	dir[directoryIndex].inode = inodeIndex;
	strcpy(dir[directoryIndex].filename,filename);
	inodes[inodeIndex].valid =1;
	inodes[inodeIndex].attributes =0;
	inodes[inodeIndex].size = size;
	// check if there isn't file system open 	
	if (currentfs == NULL)
	{
		printf("A file system is not open for the file to be placed in\n");
		return -1;
	}
	int bytes, i = 0;
	if(ret != -1)
	{
		//Read section adapted form block read example.c
		FILE *ifp = fopen(filename,"r");
		int copy_size = buf.st_size;
		int offset = 0;
		int block_index;
		while(copy_size>0)
		{
			block_index = findFreeBlock();
			inodes[inodeIndex].blocks[i] = block_index;
			i++;
			fseek(ifp,offset,SEEK_SET);
			bytes = fread(blocks[block_index], BLOCK_SIZE, 1, ifp);
			if (bytes == 0 && !feof(ifp))
			{
				printf("An error occured reading from the input file\n");
				return -1;
			}
			clearerr:(ifp);
			copy_size -= BLOCK_SIZE;
			offset+=BLOCK_SIZE;
		}
		fclose(ifp);
	}
}
void get(char* filename, char* newfname)
{
	
	int i;
	for(i=0;i<NUM_FILES;i++)
	{
		if(strcmp(filename,dir[i].filename)==0)
		{
			int inodei = dir[i].inode;
			int status,bytes;
			struct stat buf;
			if(status != -1)
			{
				FILE *ofp;
				if (newfname==NULL)
				{
					ofp = fopen(filename,"w");
				}
				else
				{
					ofp = fopen(newfname,"w");
				}
				int copy_size = inodes[inodei].size;
				int offset = 0;
				int block_index = 0;
				while(copy_size > 0)
				{
					if (copy_size<BLOCK_SIZE)
					{
						bytes = copy_size;
					}
					else
					{
						bytes = BLOCK_SIZE;
					}
					int writeindex = inodes[inodei].blocks[block_index];
					fwrite(blocks[writeindex], bytes,1,ofp);
					copy_size -= BLOCK_SIZE;
					offset += BLOCK_SIZE;
					block_index++;
					fseek(ofp,offset,SEEK_SET);
				}
				fclose(ofp);
			
				return 0;
			}
		}
	
	}
	//if this block actually ends I messed up so print that we couldnt find it
	printf("File not found\n");
	return -1;
}
void del(char* filename)
{
	int i;
	for(i=0;i<NUM_FILES;i++)
	{
		// find the file
		if(strcmp(filename,dir[i].filename)==0)
		{
			//check if it's read only
			if(inodes[dir[i].inode].attributes == 2 || inodes[dir[i].inode].attributes == 3)
			{
				printf("File is read only. It cannot be deleted.\n");
				return -1;
			}
			//This is a valid delete now so we just zero out everything that needs to be zeroed out
			dir[i].valid = 0;
			memset(dir[i].filename,0,255);
			int j = 0;
			for(j;j<1250;j++)
			{
				if (inodes[dir[i].inode].blocks[j] > 9)
				{
					//No need to memset data blocks since the read functions already ensure that 
					//we won't read already deleted bits in a data block
					//memset(blocks[inodes[dir[i].inode].blocks[j]],0,BLOCK_SIZE);
					freeBlockList[inodes[dir[i].inode].blocks[j]] = 0;
					inodes[dir[i].inode].blocks[j] = -1;
				}
			}
			inodes[dir[i].inode].valid = 0;
			inodes[dir[i].inode].attributes = 0;
			inodes[dir[i].inode].size = 0;
			freeInodeList[inodes[dir[i].inode].blocks[j]] = 0;
			dir[i].inode = -1;
			return 0;
		}
	
	}
	//if we get here something is wrong so we just slap out an error message
	printf("File not found\n");
	return -1;
}
//list function that also lists hidden files
void listh()
{
	int i, status, printed = 0;
	struct stat buf;
	// iterate through and print
	for (i=0; i<NUM_FILES;i++)
	{ 
		
		if(dir[i].valid == 1)
		{
			printed = 1;
			status = stat(dir[i].filename,&buf);	
			int inode_i = dir[i].inode;
			printf("%d %s %s", inodes[inode_i].size,dir[i].filename,ctime(&buf.st_ctime));
		}
	}
	// printed is a boolean that gets flipped when we print a valid directory
	// if this is still equal to zero then there are no valid entries
	if (printed==0)
	{
		printf("No files found\n");
	}
}
void list()
{
	
	int i, status, printed = 0;
	struct stat buf;
	for (i=0; i<NUM_FILES;i++)
	{ 
		// if valid and if not hidden
		if((dir[i].valid == 1))
		{
			// attribute = 1 is hidden, attribute = 3 is hidden and read only
			if((inodes[dir[i].inode].attributes != 1) && (inodes[dir[i].inode].attributes != 3))
			{	
				printed = 1;
				status = stat(dir[i].filename,&buf);	
				int inode_i = dir[i].inode;
				// wasn't sure how to print time but ctime seems to work
				printf("%d %s %s", inodes[inode_i].size,dir[i].filename,ctime(&buf.st_ctime));
			}
		}
	}
	// printed is a boolean that gets flipped when we print a valid directory
	// if this is still equal to zero then there are no valid entries
	if (printed==0)
	{
		printf("No files found\n");
	}
}

//Attributes section. Attribute = 1 is hidden, 2 is read only, 3 is both, 0 is neither.
void addr(char* filename)
{
	int i;
	for (i=0; i<NUM_FILES;i++)
	{ 
		if((dir[i].valid == 1));
		{
			//find file and then change attributes
				
			if ((strcmp(dir[i].filename,filename)==0))
			{
				int inode_i = dir[i].inode;
				if (inodes[inode_i].attributes == 1)
				{
					inodes[inode_i].attributes = 3;
					return 0;
				}
		        	if (inodes[inode_i].attributes == 0)
				{
					inodes[inode_i].attributes = 2;	
					return 0;
				}
			}
		}
	}
	printf("File not found\n");
	return -1;
}
void addh(char* filename)
{
	int i;
	for (i=0; i<NUM_FILES;i++)
	{ 
		if((dir[i].valid == 1));
		{	
			if ((strcmp(dir[i].filename,filename)==0))
			{
				int inode_i = dir[i].inode;
				if (inodes[inode_i].attributes == 2)
				{
					inodes[inode_i].attributes = 3;
					return 0;
				}
				if (inodes[inode_i].attributes == 0)
				{
					inodes[inode_i].attributes = 1;	
					return 0;
				}
			}
		}
	}

	printf("File not found\n");
	return -1;
}
void subr(char* filename)
{
	int i;
	for (i=0; i<NUM_FILES;i++)
	{ 
		if((dir[i].valid == 1));
		{	
			if ((strcmp(dir[i].filename,filename)==0))
			{
				int inode_i = dir[i].inode;
				if (inodes[inode_i].attributes == 3)
				{
					inodes[inode_i].attributes = 1;
					return 0;
				}
				else if (inodes[inode_i].attributes == 2)
				{
					inodes[inode_i].attributes = 0;
					return 0;	
				}
			}
		}
	}
	printf("File not found\n");
	return -1;
}
void subh(char* filename)
{
	int i;
	for (i=0; i<NUM_FILES;i++)
	{ 
		if((dir[i].valid == 1));
		{	
			
			if ((strcmp(dir[i].filename,filename)==0))
			{
				int inode_i = dir[i].inode;
				if (inodes[inode_i].attributes == 3)
				{
					inodes[inode_i].attributes = 2;
					return 0;
				}
				else if (inodes[inode_i].attributes == 1)
				{
					inodes[inode_i].attributes = 0;	
					return 0;
				}
			}
		}
	}
	printf("File not found\n");
	return -1;
}
void closefs()
{
	if (currentfs == NULL)
	{
		printf("There is no file system currently open\n");\
		return -1;
	}
	int status, bytes;
	struct stat buf;
	status = stat(currentfs, &buf);
	if (status != -1)
	{
		FILE *ofp;
		ofp = fopen(currentfs,"w");
		int block_index = 0;
		int copy_size = buf.st_size;
		int offset = 0;
		if (ofp == NULL)
		{
			printf("Could not open output file: %s\n", currentfs);
			return -1;
		}
		while(copy_size > 0)
		{
			if (copy_size<BLOCK_SIZE)
			{
				bytes = copy_size;
			}
			else
			{
				bytes = BLOCK_SIZE;
			}
			fwrite(blocks[block_index], bytes,1,ofp);
			copy_size -= BLOCK_SIZE;
			offset += BLOCK_SIZE;
			block_index++;
			fseek(ofp,offset,SEEK_SET);
		}
		fclose(ofp);
		// re initialize everything to fully close out of the file system
		currentfs = NULL;
		initializeDirectory();
		initializeBlockList();
  		initializeInodeList();
  		initializeInodes();
	}	
}
void createfs(char* filename)
{
	if (currentfs != NULL)
	{
		//auto save close so I don't lose files on accident
		closefs();
	}
	if (filename == NULL)
	{
		printf("File name not given\n");
		return -1;
	}
	fd = fopen(filename,"w");
	memset(&blocks[0],0,NUM_BLOCKS*BLOCK_SIZE);
	fwrite(&blocks[0], NUM_BLOCKS, BLOCK_SIZE, fd);
	fclose(fd);
	//strcpy(currentfs,filename);
	currentfs = filename;
}
void openfs(char* filename)
{	
	if (currentfs != NULL)
	{
		// auto save so i don't lose files on accident
		closefs();
	}
	int status,bytes;
	struct stat buf;
	status = stat(filename,&buf);
	if(status != -1)
	{
		FILE *ifp = fopen(filename,"r");
		int copy_size = buf.st_size;
		int offset = 0;
		int block_index = 0;
		while(copy_size>0)
		{
			fseek(ifp,offset,SEEK_SET);
			bytes = fread(blocks[block_index], BLOCK_SIZE, 1, ifp);
			if (bytes == 0 && !feof(ifp))
			{
				printf("An error occured reading from the input file\n");
				return -1;
			}
			clearerr(ifp);
			copy_size -= BLOCK_SIZE;
			offset+=BLOCK_SIZE;
			block_index++;
		}
		fclose(ifp);
		currentfs = filename;
		return 0;
	}
	printf("File not found\n");
	return -1;
}
int main()
{
  //initialize everything block
  dir = (struct Directory_Entry*)&blocks[0];
  inodes = (struct Inode *)&blocks[7];
  freeInodeList = (uint8_t*)&blocks[8];
  freeBlockList = (uint8_t*)&blocks[9];
 
  initializeDirectory();
  initializeBlockList();
  initializeInodeList();
  initializeInodes();
  // parsing from msh.c
  char * cmd_str = (char*) malloc( MAX_COMMAND_SIZE );
  while( 1 )
  {
    // Print out the mfs prompt
    printf ("mfs> ");

    // Read the command from the commandline.  The
    // maximum command that will be read is MAX_COMMAND_SIZE
    // This while command will wait here until the user
    // inputs something since fgets returns NULL when there
    // is no input
    while( !fgets (cmd_str, MAX_COMMAND_SIZE, stdin) );

    /* Parse input */
    char *token[MAX_NUM_ARGUMENTS];

    int   token_count = 0;                                 
                                                           
    // Pointer to point to the token
    // parsed by strsep
    char *arg_ptr;                                         
                                                           
    char *working_str  = strdup( cmd_str );                

    // we are going to move the working_str pointer so
    // keep track of its original value so we can deallocate
    // the correct amount at the end
    char *working_root = working_str;

    // Tokenize the input stringswith whitespace used as the delimiter
    while ( ( (arg_ptr = strsep(&working_str, WHITESPACE ) ) != NULL) && 
              (token_count<MAX_NUM_ARGUMENTS))
    {
      token[token_count] = strndup( arg_ptr, MAX_COMMAND_SIZE );
      if( strlen( token[token_count] ) == 0 )
      {
        token[token_count] = NULL;
      }
        token_count++;
    }
    // Now print the tokenized input as a debug check
    // \TODO Remove this code and replace with your shell functionality

    int token_index  = 0;
    for( token_index = 0; token_index < token_count; token_index ++ ) 
    {
	
	if (token[token_index]!= NULL)
	{
		//token interpretation ripped from msh.,
		if (strcmp(token[token_index], "df")==0)
		{
			printf("Disk space remaining %d\n",df());
		}
		if (strcmp(token[token_index], "list")==0)
		{
			if (token[token_index+1]!= NULL && strcmp(token[token_index+1],"-h") == 0)
			{
				listh();
			}
			else
			{
				list();
			}
		}
	 	if ((strcmp(token[token_index], "put")==0)&&(token[token_index+1]!= NULL))
		{
			put(token[token_index+1]);
		}
		if ((strcmp(token[token_index], "get")==0)&&(token[token_index+1]!=NULL))
		{
			if (token[token_index+2]!=NULL)
			{
				get(token[token_index+1],token[token_index+2]);
			}
			else
			{
				get(token[token_index+1],NULL);
			}
		}
		
		if ((strcmp(token[token_index], "del")==0)&&(token[token_index+1]!=NULL))
		{
			del(token[token_index+1]);
		}
		
		if ((strcmp(token[token_index], "attrib")==0)&&(token[token_index+1]!=NULL)&&(token[token_index+2]!=NULL))
		{
			
			if ((strcmp(token[token_index+1], "+r")==0))
			{
				addr(token[token_index+2]);
			}
			
			if ((strcmp(token[token_index+1], "+h")==0))
			{
				addh(token[token_index+2]);
			}
			if ((strcmp(token[token_index+1], "-r")==0))
			{
				subr(token[token_index+2]);
			}
			if ((strcmp(token[token_index+1], "-h")==0))
			{
				subh(token[token_index+2]);
			}
		}
		if ((strcmp(token[token_index], "open")==0)&&(token[token_index+1]!=NULL))
		{
			openfs(token[token_index+1]);
		}
		if ((strcmp(token[token_index], "create")==0)/*&&(token[token_index+1]!=NULL)*/)
		{
			createfs(token[token_index+1]);
		}
		if ((strcmp(token[token_index], "close")==0))
		{
			closefs();
		}
		
		
		
		
		
	} 
    }

    free( working_root );

  }
  return 0;
}
