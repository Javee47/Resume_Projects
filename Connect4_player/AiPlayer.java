
/**
 * This is the AiPlayer class.  It simulates a minimax player for the max
 * connect four game.
 * The constructor essentially does nothing. 
 * 
 * @author james spargo
 *
 */

public class AiPlayer 
{


    /**
     * The constructor essentially does nothing except instantiate an
     * AiPlayer object.
     *
     */
    public AiPlayer() 
    {
	// nothing to do here
    }

    /**
     * This method plays a piece randomly on the board
     * @param currentGame The GameBoard object that is currently being used to
     * play the game.
     * @return an integer indicating which column the AiPlayer would like
     * to play in.
     */
    public int findBestPlay( GameBoard currentGame, int depthLevel, int player ) 
    {


    int play = 0, ret, max = -99, maxPlay=0;
    for (play=0; play<7;play++)
    {
        if (currentGame.isValidPlay(play))
        {
            currentGame.playPiece(play);
            ret = traverse(currentGame, depthLevel, player, play, 0, max);
            currentGame.removePiece(play);
            System.out.println("Return " + ret + " for branch " + play );
            if (ret >= max)
            {
                max = ret;
                maxPlay = play;
            }
        }
    }
    return maxPlay;

    }
    public int traverse( GameBoard currentGame, int depthLevel, int player, int column, int alt, int prev)
    {
        if (depthLevel != 0 && alt == 1)
        {
            int playChoice = 0, ret, max = -99;

            for (playChoice = 0; playChoice < 7; playChoice++)
            {
                if (currentGame.isValidPlay(playChoice))
                {
                    currentGame.playPiece(playChoice);
                    ret = traverse(currentGame, depthLevel, player, playChoice, 0, max);
                    currentGame.removePiece(playChoice);
                    if (max == -99 || ret > max)
                    {
                        max = ret;
                    }
                }
            }
            return max;
        }
        else if (depthLevel != 0 && alt == 0)
        {
            int playChoice = 0, ret, min = 99;

            for (playChoice = 0; playChoice < 7; playChoice++)
            {
                if (currentGame.isValidPlay(playChoice))
                {
                    currentGame.playPiece(playChoice);
                    ret = traverse(currentGame, depthLevel-1, player, playChoice,1, min);
                    currentGame.removePiece(playChoice);
                    if (min == 99 || ret < min)
                    {
                        min = ret;
                    }
                   if (prev > min)
                   {
                       //I used this line to error check whether the alpha pruning was working or not you can uncomment it to see if it still is
                       //System.out.println("Pruned min = " + min + " prev = " + prev );
                       return min;
                   }
                }
            }
            return min;
        }
        else 
        {
            int otherPlayer;
            if (player==1)
            {
                otherPlayer =2;

            }
            else
            {
                otherPlayer =1;
            }
            return (2*currentGame.getScore(player) + currentGame.getThree(player)) - (currentGame.getScore(otherPlayer) + currentGame.getThree(otherPlayer));
        

        }
        
    }
}
