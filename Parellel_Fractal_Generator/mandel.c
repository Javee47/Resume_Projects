
#include "bitmap.h"
#include <getopt.h>
#include <stdlib.h>
#include <stdio.h>
#include <math.h>
#include <errno.h>
#include <string.h>
#include <pthread.h>
struct thread_args
{
	struct bitmap *bm;
	double xmin,xmax,ymin,ymax;
	int h,w,max,full_height,pixels;
	
};
int iteration_to_color( int i, int max );
int iterations_at_point( double x, double y, int max );
void compute_image(void *args );
pthread_mutex_t mutex;
void show_help()
{
	printf("Use: mandel [options]\n");
	printf("Where options are:\n");
	printf("-m <max>    The maximum number of iterations per point. (default=1000)\n");
	printf("-x <coord>  X coordinate of image center point. (default=0)\n");
	printf("-y <coord>  Y coordinate of image center point. (default=0)\n");
	printf("-s <scale>  Scale of the image in Mandlebrot coordinates. (default=4)\n");
	printf("-W <pixels> Width of the image in pixels. (default=500)\n");
	printf("-H <pixels> Height of the image in pixels. (default=500)\n");
	printf("-o <file>   Set output file. (default=mandel.bmp)\n");
	printf("-n set number of threads\n");
	printf("-h Show this help text.\n");
	printf("\nSome examples are:\n");
	printf("mandel -x -0.5 -y -0.5 -s 0.2\n");
	printf("mandel -x -.38 -y -.665 -s .05 -m 100\n");
	printf("mandel -x 0.286932 -y 0.014287 -s .0005 -m 1000\n\n");
}

int main( int argc, char *argv[] )
{
	char c;

	// These are the default configuration values used
	// if no command line arguments are given.
	
	const char *outfile = "mandel.bmp";
	double xcenter = 0;
	double ycenter = 0;
	double scale = 4;
	int    image_width = 500;
	int    image_height = 500;
	int    max = 1000;
	int    threadnum = 1;

	// For each command line argument given,
	// override the appropriate configuration value.

	while((c = getopt(argc,argv,"x:y:s:W:H:m:o:n:h"))!=-1) {
		switch(c) {
			case 'x':
				xcenter = atof(optarg);
				break;
			case 'y':
				ycenter = atof(optarg);
				break;
			case 's':
				scale = atof(optarg);
				break;
			case 'W':
				image_width = atoi(optarg);
				break;
			case 'H':
				image_height = atoi(optarg);
				break;
			case 'm':
				max = atoi(optarg);
				break;
			case 'o':
				outfile = optarg;
				break;
			case 'n':
				threadnum = atoi(optarg);
				break;
			case 'h':
				show_help();
				exit(1);
				break;
				
		}
	}
	// Display the configuration of the image.
	printf("mandel: x=%lf y=%lf scale=%lf max=%d outfile=%s threads=%d\n",xcenter,ycenter,scale,max,outfile,threadnum);
	
	// Create a bitmap of the appropriate size.
	struct bitmap *bm = bitmap_create(image_width,image_height);
	// Fill it with a dark blue, for debugging
	bitmap_reset(bm,MAKE_RGBA(0,0,255,0));
	
	struct thread_args *args = malloc(sizeof(struct thread_args));
	args->bm = bm;
	args->xmin = xcenter-scale;
	args->xmax = xcenter+scale;
	args->ymin = ycenter-scale;
	args->ymax = ycenter+scale;
	args->max = max;
	args->full_height = image_height;
	
 	pthread_mutex_init(&mutex,NULL);
	args->w = bitmap_width(bm)/*threadnum*/;
	args->h = bitmap_height(bm)/threadnum;
	args->pixels = args->h;
	int i=0;
	pthread_t threadarr[threadnum];
	// Compute the Mandelbrot image
	for (i=0; i<threadnum; i++)
	{
		
		pthread_create(&threadarr[i], NULL,(void*)&compute_image,(void*)args);
	}
	for (i=0;i<threadnum; i++)
	{
		pthread_join(threadarr[i],NULL);
	}
	// Save the image in the stated file.
	if(!bitmap_save(bm,outfile)) {
		fprintf(stderr,"mandel: couldn't write to %s: %s\n",outfile,strerror(errno));
		return 1;
	}

	return 0;
}

/*
Compute an entire Mandelbrot image, writing each point to the given bitmap.
Scale the image to the range (xmin-xmax,ymin-ymax), limiting iterations to "max"
*/

void compute_image(void* args)
{
	int i,j;
	struct thread_args *new_args = (struct thread_args*)args;
	pthread_mutex_lock( &mutex);
	int height = new_args->h;
	int limit = height - new_args->pixels;
	new_args->h = new_args->h + new_args->pixels;
	pthread_mutex_unlock(&mutex);

	// For every pixel in the image...

	for(j=limit;j<height;j++) {

		for(i=0;i<new_args->w;i++) {

			// Determine the point in x,y space for that pixel.
			double x = new_args->xmin + i*(new_args->xmax-new_args->xmin)/new_args->w;
			double y = new_args->ymin + j*(new_args->ymax-new_args->ymin)/new_args->full_height;
			// Compute the iterations at that point.
			int iters = iterations_at_point(x,y,new_args->max);

			// Set the pixel in the bitmap.
			bitmap_set(new_args->bm,i,j,iters);
		}
	}
	pthread_exit(NULL);
}

/*
Return the number of iterations at point x, y
in the Mandelbrot space, up to a maximum of max.
*/

int iterations_at_point( double x, double y, int max )
{
	double x0 = x;
	double y0 = y;

	int iter = 0;

	while( (x*x + y*y <= 4) && iter < max ) {

		double xt = x*x - y*y + x0;
		double yt = 2*x*y + y0;

		x = xt;
		y = yt;

		iter++;
	}

	return iteration_to_color(iter,max);
}

/*
Convert a iteration number to an RGBA color.
Here, we just scale to gray with a maximum of imax.
Modify this function to make more interesting colors.
*/

int iteration_to_color( int i, int max )
{
	int gray = 255*i/max;
	return MAKE_RGBA(gray,gray,gray,0);
}




