Joshua Venter

This a project I did for my operating systems class. I was tasked with making a custom disk image to store and retrieve files from. The original assignment template should be on https://github.com/CSE3320/Dropbox-Assignment.

It is meant to be ran on a linux system since it uses libraries from posix. It should be compilable using gcc.

The dropbox application has these commands.

1. df - which prints the current file system's remaining disk space 
2. create [name] - creates a new file system with the same name as the name argument
3. open [name] - opens a file system with the attached name.
4. close - which closes the currently open file system and saves it to the disk.
5. put [filename] - places a file with the attached name into the current file system
6. get [filename] - retrieves a file with the attached name from the current file system
7. del [filename] - deletes a file with the attached name from the current file system. Ignores read only files.
8. list [-h] - lists all the files in the current file system. Ignores hidden files unless given the argument -h.
9. attrib[+ or -][h or r] [filename] - removes or adds (depending on the + or -) either the hidden or read only attribute to the attached file name in the file system.





