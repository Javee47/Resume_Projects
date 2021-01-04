// Venter, Joshua B.
// jbv2218
// 2019-11-21


import java.util.function.UnaryOperator;
import java.util.function.BinaryOperator;

public class hmwk_05_lambda {
  public static double EPSILON = 1e-8;

  //----------------------------------------------------------
  // Babylonian cube root lambda goes here.
  static BinaryOperator<Double>
  babylonian = (Double n, Double g) -> Math.abs(n - (Math.pow(g, 3))) < EPSILON ? g : hmwk_05_lambda.babylonian.apply(n,((2*g)/3)+((n/3)/(g*g)));
 
  // pseudoPell lambda goes here.
  static UnaryOperator<Long>
  pell = ( Long i ) -> i == 0 ? 0 : i == 1 ? 1 : i==2 ? 2 : 3*hmwk_05_lambda.pell.apply((long)(i-1)) + 2*hmwk_05_lambda.pell.apply((long)(i-2)) + hmwk_05_lambda.pell.apply((long)(i-3));
  // GCF lambda goes here.
  static BinaryOperator<Long>
  gcf = (Long m, Long n ) -> m==n ? m : m>n ? hmwk_05_lambda.gcf.apply(m-n,n) : hmwk_05_lambda.gcf.apply(m,n-m); 
  
  static BinaryOperator<Integer>
    add = ( Integer i, Integer j ) -> i+j;

  //----------------------------------------------------------
  public static void main( String[] args )
  {
    double output;
    // Put for loop here that applies the Babylonian cube root lambda.
    for (int i = 0; i<341; i+=17)
    {
      output = babylonian.apply((double)i,EPSILON);
      System.out.format("babylonian(%3d) is %.6f = %10.6f%n", i, output, Math.pow(output, 3));
    }
    // Put for loop here that applies the pseudoPell lambda.
    for (int i = 0; i<21;i++)
    {
        System.out.format("pseudoPell(%d) is %d\n",i, pell.apply((long)i));
    }
    for (int m = 1; m<11;m++)
    {
      for(int n = 1; n<11;n++)
      {
        System.out.format("GCF(%d,%d) is %d\n",m,n, gcf.apply((long)m,(long)n));
      }
    }

    // Put for loop here that applies the GCF lambda.
  }
}
