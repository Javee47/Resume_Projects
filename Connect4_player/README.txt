Joshua Venter

This a project I did for my artificial intelligence class. If you want to look at the oringinal assignment it should be on his website http://crystal.uta.edu/~gopikrishnav/classes/2020/fall/4308_5360/assmts/optassmt1.html 
there you should find the original template programs he provided to see how it compares to my finished assignment or if that is no longer working I dowloaded the html file and you should be able to at least read it in my folder. 
It's labelled "Optional Assignment 1.html". The author of the original template he gave us seems to be someone named James Spargo. For full transperency, I've left most of his comments and a lot of his code untouched. I did most of my work in the AiPlayer file 
but I did make some changes to the others and added one additional heuristic function for the utility calculation called getThree.

My evaluation function works by constructing and traversing a min max until a specified depth limit. At which point it checks the current game state and it calculates how "good" that state is by
(number of 4 in a rows + number of 3 in a rows) - ( number of 4 in a rows for opponent + number of 3 in a rows for opponent).
I check the number of three in a rows with a function I added to the gameboard class. I basically just copied the 4 in a row check but removed checking the fourth index from each for loop and named the function getThree.

This should be able to run on anything that has java installed. 

The program has two modes. one move mode in which it reads in a file with a the current boardstate and outputs a single chosen move for that boardstate and interactive in which you actively play against the bot in the terminal.
I copied these next lines on how to use the program from the original template program's comments. I highly recommended booting the game with input1.txt in interactive mode to actively play against my bot to see if you can beat it.


To compile the program, use the following command from the maxConnectFour directory:
javac *.java
 the usage to run the program is as follows:
 ( again, from the maxConnectFour directory )
 
   -- for interactive mode:
  java MaxConnectFour interactive [ input_file ] [ computer-next / human-next ] [ search depth]
 
  -- for one move mode
  java maxConnectFour.MaxConnectFour one-move [ input_file ] [ output_file ] [ search depth]
  
  description of arguments: 
   [ input_file ]
   -- the path and filename of the input file for the game
   
   [ computer-next / human-next ]
   -- the entity to make the next move. either computer or human. can be abbreviated to either C or H. This is only used in interactive mode
   
   [ output_file ]
   -- the path and filename of the output file for the game.  this is only used in one-move mode
    [ search depth ]
      -- the depth of the minimax search algorithm