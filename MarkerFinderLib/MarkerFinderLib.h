#define uint unsigned int
#define PI 3.14159265358979323846
#define SCALE_MULT 25
#define TRESHOLD1 50000
#define TRESHOLD2 1300
#define SAMPLE_COUNT 32
#define MIN_CONTOUR_LENGTH 12

struct MyImage
{
	uint* img;
	int w;
	int h;	
};

struct MyImageListElement
{
	MyImage image;
	MyImageListElement* next;
	MyImageListElement()
	{
	}
};

struct MyPoint
{
	int X;
	int Y;

	MyPoint(int aX = 0, int aY = 0)
		: X(aX)
		, Y(aY)
	{	
	}
};

struct MyContourPoint
{
	MyPoint pos;
	MyContourPoint* next;
	MyContourPoint()
	{
	}
};

struct MyShape
{
	MyContourPoint* contour;
	MyPoint pos;
	double scale;
	double rot;
	int index;

	MyShape()
		: scale(1)
		, rot(0)
		, index(-1)
	{
	}
};

struct MyShapeListElement
{
	MyShape shape;
	MyShapeListElement* next;

	MyShapeListElement()
	{
	}
};

struct MyComplexData
{
	double* data;
	int length;
};

extern "C" __declspec(dllexport) void create_image_list_N(MyImageListElement** element);
extern "C" __declspec(dllexport) void add_image_list_item_N(MyImageListElement** element, MyImage image);
extern "C" __declspec(dllexport) void delete_image_list_D(MyImageListElement* list);
extern "C" __declspec(dllexport) void delete_contour_D(MyContourPoint* contour);
extern "C" __declspec(dllexport) void delete_shape_list_D(MyShapeListElement* shapeList);
extern "C" __declspec(dllexport) void clone_contour_N(MyContourPoint* src, MyContourPoint** dest);
extern "C" __declspec(dllexport) void clone_shape_N(MyShape src, MyShape* dest);
extern "C" __declspec(dllexport) void clone_image_N(MyImage src, MyImage* dest);
extern "C" __declspec(dllexport) void delete_image_D(MyImage im);
extern "C" __declspec(dllexport) void dilatation(MyImage im);
extern "C" __declspec(dllexport) void erosion(MyImage im);
extern "C" __declspec(dllexport) void opening(MyImage im);
extern "C" __declspec(dllexport) void closing(MyImage im);
extern "C" __declspec(dllexport) void substract(MyImage im1, MyImage im2);
extern "C" __declspec(dllexport) void contour(MyImage im);
extern "C" __declspec(dllexport) void clear_blob(MyImage im, MyPoint point);
extern "C" __declspec(dllexport) void segment_background_HSL(MyImage imFore, MyImage imBack, uint treshold);
extern "C" __declspec(dllexport) void segment_background(MyImage imFore, MyImage imBack, uint treshold);
extern "C" __declspec(dllexport) void segment_kmeans(MyImage im, int levels);
extern "C" __declspec(dllexport) void binarize(MyImage im, uint color0, uint color1);
extern "C" __declspec(dllexport) void binarize2(MyImage im, bool inverse);
extern "C" __declspec(dllexport) void remove_outside(MyImage im, MyContourPoint* contour, uint clearColor);
extern "C" __declspec(dllexport) void draw_line(MyImage im, MyPoint p1, MyPoint p2, uint color);
extern "C" __declspec(dllexport) void draw_shape_lines(MyImage im, MyContourPoint* contour, MyPoint pos, uint color);
extern "C" __declspec(dllexport) void draw_shape(MyImage im, MyContourPoint* contour, MyPoint pos, uint color);
extern "C" __declspec(dllexport) void draw_shape_list(MyImage im, MyShapeListElement* shList);
extern "C" __declspec(dllexport) void real_contour_N(MyImage im, MyShapeListElement** shapeList);
extern "C" __declspec(dllexport) void to_complex_N(MyContourPoint* contour, MyComplexData* out);
extern "C" __declspec(dllexport) void to_contour_N(MyComplexData complexData, MyContourPoint** contour);
extern "C" __declspec(dllexport) void resample_N(MyComplexData src, int destCount, MyComplexData* dest);
extern "C" __declspec(dllexport) void FFT(MyComplexData indata, int sign);
extern "C" __declspec(dllexport) void normalize_shape(MyShape* shape);
extern "C" __declspec(dllexport) void generate_marker_shapes_N(MyImageListElement* markers);
extern "C" __declspec(dllexport) void match_shape(MyShape* shape, MyShapeListElement* markers);
extern "C" __declspec(dllexport) void match_shape2(MyShape* shape, MyShapeListElement* markers);
extern "C" __declspec(dllexport) void find_objects_N(MyImage foreground, MyImage background, MyShapeListElement** shapeList);

extern "C" __declspec(dllexport) MyImage readImage(char* filename, int w, int h);
extern "C" __declspec(dllexport) void writeImage(char* filename, MyImage im);
extern "C" __declspec(dllexport) void writeImageRGB(char* filename, MyImage im) ;