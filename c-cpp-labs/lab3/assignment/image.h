#ifndef _IMAGE
#define _IMAGE

#define DEFAULT_IMAGE_WIDTH 638
#define DEFAULT_IMAGE_HEIGHT 533

typedef struct {
	int width;
	int height;
	unsigned char*** data;
} image;

typedef struct {
	int x;
	int y;
} pixel;


// initializes the image
// given the width and height
void init_image(image* im, int width, int height);

// initializes the image
// with default settings
void init_image_default(image* im);

// frees the allocated memory of the matrix
void free_image(image* im);

// inverts all color elements a rectancle in the image
// creating the negative of that rectantle
// the rectangle is defined by the pixels from and to
void negative_image(image* im, pixel* from, pixel* to);

// flips a rectancle in the image vertically
// the rectangle is defined by the pixels from and to
void flip_image_vertically(image* im, pixel* from, pixel* to);

// flips a rectancle in the image horizontally
// the rectangle is defined by the pixels from and to
void flip_image_horizontally(image* im, pixel* from, pixel* to);


// resizes image dynamically to size new_width x new_height
// possible new pixels will be initialized with r, g and b
void dynamic_resize_image (image* im, int new_width, int new_height, unsigned char r, unsigned char g, unsigned char b);

// stretches an image to size new_width x new_height
void stretch_image (image* im, int new_width, int new_height);

// extracts the least significant (rightmost) bit from the input char
// shifts it to the left and places it in the output byte.
void extractLeastSignificantBit(unsigned char input, int shift, unsigned char* output);

// decodes message from an image into the preinitialized char buffer
// max_len is the size reserverd for the message including the 0x00 termination byte
// so the total of bytes reserved for the message
void decode_message (image* im, char* message, int max_len);

// takes from the input char, the bit according the bitIndex [0-7]
// and writes this to the least significant (rightmost) bit of the output, 
void writeLeastSignificantBit(unsigned char input, int bitIndex, unsigned char* output);

// encodes message into an image
void encode_message (image* im, char* message);

// loads an image structure from file
// do not touch!
void load_image(image* im, char* file_name);

// saves the image to a file
// do not touch!
void save_image(image* im, char* file_name);

#endif