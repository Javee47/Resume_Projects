// Venter, Joshua B.
// jbv2218
// 2019-11-21

 
public class hmwk_05_imperative {
  public static double EPSILON = 1e-8;
 
  //----------------------------------------------------------
  // Babylonian cube root
  public static double babylonian( final double number, double guess) {
    final double check = number - (Math.pow(guess, 3));
    double newGuess;
    if (Math.abs(check)<EPSILON)
    {
      return guess;
    }
    double g1 = ((2*guess)/3);
    double g2 = ((number/3)/(guess*guess));
    newGuess = g1 + g2;
    return babylonian( number, newGuess);
   
  }
 
  // Pseudo-Pell numbers
  public static Long pseudoPell(final Long n) {
    if (n==0)
    {
      return (long)0;
    }
    else if (n==1)
    {
      return (long)1;
    }
    else if(n==2)
    {
      return (long)2;
    } 
    
    return 3*pseudoPell(n-1) + 2*pseudoPell(n-2) + pseudoPell(n-3);
  }
 
  // GCF
  public static Long GCF(final Long m, final Long n) {
    if (m==n)
    {
      return m;
    }
    else if (m>n)
    {
      return GCF(m-n,n);
    }
    else
    {
      return GCF(m,n-m);
    }
  }
 
  // ----------------------------------------------------------
  public static void main(final String[] args)
  {
    double output;
    long pellouput;
    long gcfoutput;
    for (int i = 0; i<341; i+=17)
    {
      output=babylonian(i, EPSILON);
      System.out.format("babylonian(%3d) is %.6f = %10.6f%n", i, output, Math.pow(output, 3));
    }
    for (int i = 0; i<21; i++)
    {
      pellouput=pseudoPell((long)i);
      System.out.println("pseudoPell(" + i + ") is " + pellouput);
    }
 
    for (int m = 1; m<11; m++)
    {
        for(int n = 1; n<11; n++)
        {
            gcfoutput = GCF((long)m, (long)n);
            System.out.println("GCF(" + m +", "+ n + ") is " + gcfoutput);
        }
    }
  }
 }
 


