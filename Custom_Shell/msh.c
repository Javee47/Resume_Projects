// msh.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

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
#include <unistd.h>
#include <sys/wait.h>
#include <stdlib.h>
#include <errno.h>
#include <string.h>
#include <signal.h>

#define WHITESPACE " \t\n"      // We want to split our command line up into tokens
								// so we need to define what delimits our tokens.
								// In this case  white space
								// will separate the tokens on our command line

#define MAX_COMMAND_SIZE 255    // The maximum command-line size

#define MAX_NUM_ARGUMENTS 55     // Mav shell only supports five arguments

// Joshua Venter
// 1001462218
// READ.ME
// the !0 command only works with a space between the ! and the 0 like ! 0
// I'm not sure if the child suspension works correctly but if it doesn't it's at least close to working
// the ! 0 command also accepts arguments after it until the next semicolon. So ! 0 -a actually works.

void sigh(int sig)
{ 
	//signal handler for parent just catches functions

}
void childsignals(int sig)
{
	// signal handler for child. Pauses function if SIGTSTP. Wakes function if SIGALRM
	if(SIGTSTP)
	{
		pause();
	}
	
	
}

int main()
{

	char* cmd_str = (char*)malloc(MAX_COMMAND_SIZE);
	//initialize last 15 pids array
	
	pid_t pid_list[15];
	int pid_count =0;
	pid_t child_pid;
	//initialize signal hanlder for parent to catch int and stop
	struct sigaction act;
	memset(&act,'\0', sizeof(act));
	

	act.sa_handler =&sigh;
	if (sigaction(SIGINT, &act, NULL ) < 0)
	{
		perror("sigaction: ");
		return 1;
	}

	if (sigaction(SIGTSTP, &act, NULL) < 0)
	{
		perror("sigaction: ");
		return 1;
	} 

	//initialize command history array:
	char** history_list = malloc(sizeof(char*)*50);
	int history_index = 0;
	for(history_index;history_index<50;history_index++)
	{
		*(history_list+history_index) == malloc(MAX_COMMAND_SIZE);	
	}
	history_index = 0;
	int i =0;
	while (1)
	{
		// Print out the msh prompt
		printf("msh> ");
		
		
		// Read the command from the commandline.  The
		// maximum command that will be read is MAX_COMMAND_SIZE
		// This while command will wait here until the user
		// inputs something since fgets returns NULL when there
		// is no input
		while (!fgets(cmd_str, MAX_COMMAND_SIZE, stdin));

		/* Parse input */
		char* token[MAX_NUM_ARGUMENTS];

		int   token_count = 0;

		// Pointer to point to the token
		// parsed by strsep
		char* arg_ptr;

		char* working_str = strdup(cmd_str);

		// we are going to move the working_str pointer so
		// keep track of its original value so we can deallocate
		// the correct amount at the end
		char* working_root = working_str;

		// Tokenize the input stringswith whitespace used as the delimiter
		while (((arg_ptr = strsep(&working_str, WHITESPACE)) != NULL) &&
			(token_count < MAX_NUM_ARGUMENTS))
		{
			token[token_count] = strndup(arg_ptr, MAX_COMMAND_SIZE);
			if (strlen(token[token_count]) == 0)
			{
				token[token_count] = NULL;
			}
			token_count++;
		} 
		// char recprd is used by history at the bottom to keep track of what token[token_index] originally was to record it
		char* record;

		// Iterate through tokenized array and execute commands

		int token_index = 0;
		// mew command next is just a boolean basically that sorts through the input to see if the next token is a argument for a command or a new command
		int new_command_next = 1;
		//checks if input is just whitespace
			for (token_index; token_index < token_count; token_index++)
			{
				if (token[token_index] == NULL)
				{
					break;
				}
				// new_command_next = 1 when the next token is the first token ever reiceived or if its the first token after a ;. Other wise it ignores the input.
				if (new_command_next == 1)
				{
					

					// check if a ! 0 comand was input
					if (strcmp(token[token_index], "!") == 0) 
					{
						// go past exlamation point since its no longer needed
						token_index++;
						// get the number next to the !
						if (token[token_index] != NULL)
						{
							// histint = command being recalled
							int histint = atoi(token[token_index]);
							// check if the index is wrong
							if (*(history_list + histint) != NULL )
							{
								// make the current index the copied command and let the rest of the program execute normally
								token[token_index] = strdup(*(history_list+histint));
							}
							else
							{
								printf("Invalid history index\n");
							} 
						}
						
					}
					//record what the command was before its altered by my utter mess of a code block
					record = strdup(token[token_index]);
					if ((strcmp(token[token_index],"quit") == 0)||(strcmp(token[token_index],"exit")==0))
					{
						return 0;
					}
					else if(strcmp(token[token_index], "bg") ==0)
					{
							// attempt to wake up process
							kill(child_pid, SIGALRM);
						
					}
					else if (strcmp(token[token_index],"history") == 0)
					{
						i = 0;
						for (i;i<history_index;i++)
						{
							printf("%d: %s\n", i, *(history_list+i));
						}	
					}
					else if (strcmp(token[token_index],"listpids")==0)
					{
						i = 0; 
						for (i; i<pid_count; i++)
						{
							printf("%d: %ld%ld\n", i, pid_list[i]);			

						}
					}
					else if (strcmp(token[token_index],"cd")==0)
					{
						chdir(token[++token_index]);
					}
					else 
					{
					
						child_pid = fork();
					
						int status;
						//Determine which is the child
						if (child_pid == 0)
						{     
							// install alrm and stop onto the child
							// if i did this right stop should force the child to pause
							// and sig alarm should wake it up because a handled signal wakes up a paused process
							struct sigaction child_act;
							memset(&child_act,'\0',sizeof(child_act));
							child_act.sa_handler =& childsignals;
							if(sigaction(SIGTSTP, &child_act, NULL))
							{
								perror("sigaction: ");
								return 0;
							}
							if(sigaction(SIGALRM, &child_act, NULL))
							{
								perror("sigaction: ");
								return 0;
							}

							//make a copy of token_index since I will manipulate to look for arguments to pass later
							// and i need it still for the exec calls
							int temp = token_index;	
							
						   
							//make list of arguments. one additional index for a NULL terminator 
							char* args[11];
							// args interator. set to one since the first index will always be the command itself
							int args_index = 1;
							// copy the command into args
							args[0] = strndup(token[token_index], MAX_COMMAND_SIZE);
							// memset the rest to NULL 
							i = 1;
							for (i;i<11;i++)
							{
								args[i] = NULL;
							}
							//check next index for an argument to be passed to the command
							token_index++;
							while((token_index<token_count)&&(token[token_index] != NULL)&&(strcmp(token[token_index],";") != 0))
							{
								args[args_index] = token[token_index];
								token_index++;
								args_index++;
							}
							// create a new array in the format the exec functions accept and copy all values in arg over
							char * const passargs[11] = {args[0], args[1], args[2], args[3],args[4],args[5],args[6],args[7],args[8],args[9], args[10]};



							//create addresses we will search for executables
							char usrlocalbin[] = "/usr/local/bin/";
							char usrbin[] = "/usr/bin/";
							char bin[] = "/bin/";
							strcat(usrlocalbin,token[temp]);
							strcat(usrbin,token[temp]);
							strcat(bin,token[temp]);
					
							execvp(token[temp],passargs);
											
							execv(usrlocalbin,passargs);

							execv(usrbin,passargs);

							execv(bin,passargs);
							// use error message from exec
							if (strerror(errno) != NULL)
							{
								printf("%s\n", strerror(errno));
							}
							else
							{
								printf("Command not found:\n");
							}
			
							return 1;
				
						}
						//make the child execute first	
						waitpid(child_pid,&status,0);
						//pause();
						//PID list logic
						if (pid_count != 15)
						{
							//if PID list isn't full then insert new pid and ++ the index counter
							pid_list[pid_count] == child_pid;
							pid_count++;
						
						}
						else
						{
							// if PID list is full. move all the values down by one and replace the oldest value with new pid
							i = 0;
							for (i;i<14;i++)
							{
								pid_list[i] = pid_list[i+1];
							}
							pid_list[14] = child_pid;
						}	
					}
					//Command history logic
					if (history_index != 50)
					{
						// insert new command
						*(history_list+history_index) = strdup(record);
						history_index++;
					}
					else
					{
						// if history is full move all values down by one and replace oldest value with new command
						i = 0;
						for(i;i<49;i++)
						{
							*(history_list+i) = strdup(*(history_list+i+1));		
						}
						*(history_list+49) = strdup(record);
					
					}
					new_command_next = 0;
				}
				//see if the next token is a semicolon to determine if the next value is a command
				else if (strcmp(token[token_index], ";") == 0)
				{
					new_command_next = 1;
				}
			
		}

		free(working_root);

	}
	return 0;
}
