// MarkerFinderLib.cpp : Defines the exported functions for the DLL application.
//

#include "MarkerFinderLib.h"
#include <list>
#include <queue>
#include <vector>
#include <math.h>
#include <iostream>
#include <fstream>

using namespace std;


MyShapeListElement *globalMarkerShapeList;

void create_image_list_N(MyImageListElement** element) {
	if (element) {
		(*element) = new MyImageListElement();
		(*element)->next = NULL;
	}
}

void add_image_list_item_N(MyImageListElement** element, MyImage image) {
	if (element) {
		MyImageListElement* newelement = new MyImageListElement();
		newelement->image = image;
		MyImageListElement* last = NULL;
		MyImageListElement* current = *element;
	
		while(current->next) {
			last = current;
			current = current->next;
		}
		newelement->next = current;
		if (last) last->next = newelement;	
		else (*element) = newelement;	
	}
}

void delete_image_list_D(MyImageListElement* list) {
	if (list) {	
		MyImageListElement* it = list;	
		while(it->next) {
			MyImageListElement* now = it;
			it = it->next;
			delete now;
		}
		delete it;		
	}
}
void delete_contour_D(MyContourPoint* contour) {
	if (contour) {
		MyContourPoint* it = contour;	
		while(it->next) {
			MyContourPoint* now = it;
			it = it->next;
			delete now;
		}
		delete it;		
	}
}

void delete_shape_list_D(MyShapeListElement* shapeList) {
	if (shapeList) {
		MyShapeListElement* it = shapeList;	
		while(it->next) {
			MyShapeListElement* now = it;
			it = it->next;
			delete_contour_D(now->shape.contour);
			delete now;
		}
		delete it;		
	}
}

void clone_contour_N(MyContourPoint* src, MyContourPoint** dest) {		
	if (src && dest) {
		MyContourPoint* it = src;	
		MyContourPoint* it2 = (*dest) = new MyContourPoint();	
		while(it->next) {
			it2->pos = it->pos;
			it2->next = new MyContourPoint();
			it2 = it2->next;			
			it = it->next;			
		}
		it2->next = NULL;	
	}
}


void clone_shape_N(MyShape src, MyShape* dest) {		
	if (dest) {
		(*dest).index = src.index;
		(*dest).pos = src.pos;
		(*dest).rot = src.rot;
		(*dest).scale = src.scale;		
		clone_contour_N(src.contour,&((*dest).contour));
	}	
}

void clone_image_N(MyImage src, MyImage* dest) {
	if (dest) {
		dest->w = src.w;
		dest->h = src.h;
		dest->img = new uint[src.w * src.h];
		for(int i = 0; i < src.w * src.h; ++i) {
			dest->img[i] = src.img[i];
		}
	}
}

void delete_image_D(MyImage im) {
	if (im.img) delete[] im.img;
}


void dilatation(MyImage im)
{	
	int w = im.w;
	int h = im.h;
	uint* img = im.img;
	if (img) {
		uint* ret = new uint[w * h];
		for (int x = 1; x < w - 1; ++x)
		{
			for (int y = 1; y < h - 1; ++y)
			{
				bool match = false;
				for (int dx = -1; dx <= 1; ++dx)
				{
					for (int dy = -1; dy <= 1; ++dy)
					{
						if (img[x + dx + (y + dy) * w]) match = true;
					}
				}
				if (match) ret[x + y * w] = 1;
				else ret[x + y * w] = 0;
			}
		}

		for(int i = 0; i < w; ++i) 
		{
			ret[i + 0 * w] = 0;
			ret[i + (h-1) * w] = 0;
		}

		for(int i = 0; i < h; ++i) 
		{
			ret[0 + i * w] = 0;
			ret[w-1 + i * w] = 0;
		}

		for (int i = 0; i < w * h; ++i)
		{
			img[i] = ret[i];
		}   

		delete[] ret;
	}
}

void erosion(MyImage im)
{
	int w = im.w;
	int h = im.h;
	uint* img = im.img;
	if (img) {
		uint* ret = new uint[w * h];
		for (int x = 1; x < w - 1; ++x)
		{
			for (int y = 1; y < h - 1; ++y)
			{
				bool match = false;
				for (int dx = -1; dx <= 1; ++dx)
				{
					for (int dy = -1; dy <= 1; ++dy)
					{
						if (!img[x + dx + (y + dy) * w]) match = true;
					}
				}
				if (match) ret[x + y * w] = 0;
				else ret[x + y * w] = 1;
			}
		}

		for(int i = 0; i < w; ++i) 
		{
			ret[i + 0 * w] = 0;
			ret[i + (h-1) * w] = 0;
		}

		for(int i = 0; i < h; ++i) 
		{
			ret[0 + i * w] = 0;
			ret[w-1 + i * w] = 0;
		}

		for (int i = 0; i < w * h; ++i)
		{
			img[i] = ret[i];
		} 

		delete[] ret;
	}
}

void opening(MyImage im)
{
	erosion(im);
	dilatation(im);
}

void closing(MyImage im)
{
	dilatation(im);
	erosion(im);
}

void RGB2HSL (uint rgb, double* h, double* s, double* l)
{
	
	double r = ((unsigned char*)&rgb)[2];
	double g = ((unsigned char*)&rgb)[1];					
	double b = ((unsigned char*)&rgb)[0];
	
	double v;
	double m;
	double vm;
	double r2, g2, b2;

	*h = 0; // default to black
	*s = 0;
	*l = 0;
	v = max(r,g);
	v = max(v,b);
	m = min(r,g);
	m = min(m,b);
	*l = (m + v) / 2.0;
	if (*l <= 0.0) 
	{
		return;
	}
	vm = v - m;
	*s = vm;
	if (*s > 0.0) 
	{
		*s /= (*l <= 0.5) ? (v + m ) : (2.0 - v - m) ;
	} 
	else 
	{
		return;
	}
	r2 = (v - r) / vm;
	g2 = (v - g) / vm;
	b2 = (v - b) / vm;
	if (r == v) 
	{
		*h = (g == m ? 5.0 + b2 : 1.0 - g2);
	}
	else if (g == v) 
	{
		*h = (b == m ? 1.0 + r2 : 3.0 - b2);
	}
	else 
	{
		*h = (r == m ? 3.0 + g2 : 5.0 - r2);
	}
	*h /= 6.0;
}


void substract(MyImage im1, MyImage im2) 
{   

	if ((im1.w == im2.w) && (im1.h == im2.h) && im1.img && im2.img)
	{
		for (int i = 0; i < im1.w * im1.h; ++i)
		{
			if ((im1.img[i]) && (!im2.img[i])) im1.img[i] = 1;
			else im1.img[i] = 0;
		}
	}
}

void contour(MyImage im)
{
	MyImage im2;
	clone_image_N(im,&im2);
	dilatation(im);
	substract(im, im2);   
	delete_image_D(im2);
}

void clear_blob(MyImage im, MyPoint point)		
{
	queue<MyPoint> pointQueue;
	uint* img = im.img;
	int xx = point.X;
	int yy = point.Y;
	int w = im.w;
	int h = im.h;	
	if (img) {
		pointQueue.push(MyPoint(xx, yy));
		img[xx + yy * w] = 0;

		while (!pointQueue.empty())
		{
			const MyPoint p = pointQueue.front();		
			const int& x = p.X;
			const int& y = p.Y;

			pointQueue.pop();

			if ((x - 1 >= 0) && (img[x - 1 + y * w] != 0))
			{                        
				img[x - 1 + y * w] = 0;
				pointQueue.push(MyPoint(x - 1, y));                        
			}

			if ((x + 1 < w) && (img[x + 1 + y * w] != 0))
			{
				img[x + 1 + y * w] = 0;
				pointQueue.push(MyPoint(x + 1, y)); 
			}

			if ((y - 1 >= 0) && (img[x + (y - 1) * w] != 0))
			{
				img[x + (y - 1) * w] = 0;
				pointQueue.push(MyPoint(x, y - 1)); 
			}

			if ((y + 1 < h) && (img[x + (y + 1) * w] != 0))
			{
				img[x + (y + 1) * w] = 0;
				pointQueue.push(MyPoint(x, y + 1)); 
			}
		}    
	}
}

void segment_background_HSL(MyImage imFore, MyImage imBack, uint treshold)
{
	uint* img = imFore.img;
	int w = imFore.w;
	int h = imFore.h;		
	uint* bg = imBack.img;
	int bgw = imBack.w;
	int bgh = imBack.h;
	if (img && bg) {
		int* dif = new int[w * h];

		for(int i=0; i<w*h; ++i) 
		{
			uint c1 = img[i];
			uint c2 = bg[i];

			double h1,s1,l1,h2,s2,l2;
			RGB2HSL(c1, &h1, &s1, &l1);
			RGB2HSL(c2, &h2, &s2, &l2);
			
			int dh = (h1 - h2) *255.0;
			int ds = (s1 - s2) *255.0;
			int dl = (l1 - l2) *255.0;
			dif[i] =  (dh * dh + ds * ds);			
		}

		for (int x = 1; x < w - 1; ++x)
		{
			for (int y = 1; y < h - 1; ++y)
			{
				uint diff = 0;
				for (int dx = -1; dx <= 1; ++dx)
				{
					for (int dy = -1; dy <= 1; ++dy)
					{
						diff += dif[x + dx + (y + dy) * w];
					}
				}
				if (diff < treshold) img[x + y * w] = 0;
				else img[x + y * w] = 1;
			}
		}  

		delete[] dif;
	}
}



void segment_background(MyImage imFore, MyImage imBack, uint treshold)
{
	uint* img = imFore.img;
	int w = imFore.w;
	int h = imFore.h;		
	uint* bg = imBack.img;
	int bgw = imBack.w;
	int bgh = imBack.h;
	if (img && bg) {
		int* dif = new int[w * h];

		for(int i=0; i<w*h; ++i) 
		{
			uint c1 = img[i];
			uint c2 = bg[i];

			unsigned char a1 = ((unsigned char*)&c1)[3];
			unsigned char r1 = ((unsigned char*)&c1)[2];
			unsigned char g1 = ((unsigned char*)&c1)[1];					
			unsigned char b1 = ((unsigned char*)&c1)[0];

			unsigned char a2 = ((unsigned char*)&c2)[3];					
			unsigned char r2 = ((unsigned char*)&c2)[2];
			unsigned char g2 = ((unsigned char*)&c2)[1];
			unsigned char b2 = ((unsigned char*)&c2)[0];

			int dr = (r1 - r2);
			int dg = (g1 - g2);
			int db = (b1 - b2);
			dif[i] =  dr * dr + dg * dg + db * db;
		}

		for (int x = 1; x < w - 1; ++x)
		{
			for (int y = 1; y < h - 1; ++y)
			{
				uint diff = 0;
				for (int dx = -1; dx <= 1; ++dx)
				{
					for (int dy = -1; dy <= 1; ++dy)
					{
						diff += dif[x + dx + (y + dy) * w];
					}
				}
				if (diff < treshold) img[x + y * w] = 0;
				else img[x + y * w] = 1;
			}
		}  

		delete[] dif;
	}
}



void segment_kmeans(MyImage im, int levels)
{
	uint* img = im.img;
	int w = im.w;
	int h = im.h;		
	if (img) {
		int* means = new int[levels * 4];		
		uint* ret = new uint[w * h];

		//"veletlenszeru" besorolas

		for (int i = 0; i < w * h; ++i)
		{
			ret[i] = i % levels;                
		}


		int changed;
		do
		{
			//nullazas
			for (int j = 0; j < levels; ++j)
			{
				for (int i = 0; i < 4; ++i)
				{
					means[j * 4 + i] = 0;
				}
			}

			//kozeppontok szamitasa a besorolasok alapjan
			for (int i = 0; i < w * h - 7; i += 8)
			{
				uint j = ret[i];

				uint c1 = img[i];
							
				unsigned char a1 = ((unsigned char*)&c1)[3];
				unsigned char r1 = ((unsigned char*)&c1)[2];
				unsigned char g1 = ((unsigned char*)&c1)[1];					
				unsigned char b1 = ((unsigned char*)&c1)[0];
				
				if (a1 == 0xFF)//ha alpha nem FF, akkor nem vesszuk figyelembe
				{
					means[j * 4 + 0] += r1;
					means[j * 4 + 1] += g1;
					means[j * 4 + 2] += b1;
					means[j * 4 + 3]++;
				}
			}

			//atlagolas osztasa
			for (int j = 0; j < levels; ++j)
			{
				if (means[j * 4 + 3] != 0)
				{
					for (int i = 0; i < 3; ++i)
					{
						means[j * 4 + i] /= means[j * 4 + 3];
					}
				}
			}


			//pontok ujra besorolasa
			changed = 0;
			for (int i = 0; i < w * h; ++i)
			{
				uint minj = 0;
				uint min = 0xFFFFFFFF;
				for (int j = 0; j < levels; ++j)
				{
					uint c1 = img[i];
					uint a1 = (c1 >> 24) % 256;
					if (a1 == 0xFF)//csak akkor soroljuk be, ha FF az alpha ertek, amugy fixen levels lesz;
					{
						uint r1 = ((unsigned char*)&c1)[2] - means[j * 4 + 0];
						uint g1 = ((unsigned char*)&c1)[1] - means[j * 4 + 1];
						uint b1 = ((unsigned char*)&c1)[0] - means[j * 4 + 2];

						uint dist = r1 * r1 + b1 * b1 + g1 * g1;

						if (dist < min)
						{
							min = dist;
							minj = j;
						}
					}
					else minj = levels;
				}

				if (ret[i] != minj) changed++;
				ret[i] = minj;

			}
		}
		while (changed > w*h / 10);

		for (int i = 0; i < w * h; ++i)
		{
			int j = (int)ret[i];			
			if (j < levels) img[i] = 0xFF000000 + 0x00010000 * means[j * 4 + 0] + 0x00000100 * means[j * 4 + 1] + 0x00000001 * means[j * 4 + 2];
			else img[i] = 0x00000000;
		}    

		delete[] ret;
		delete[] means;
	}
}

void binarize(MyImage im, uint color0, uint color1)
{
	uint* img = im.img;
	int w = im.w;
	int h = im.h;		
	if (img) {

		uint cA, cB;
		cA = cB = 0;
		int j = 0;
		while((cB == 0) && (j < w*h))
		{                
			uint c = img[j];
			uint a = (c >> 24) % 256;
			if (a == 0xFF)
			{
				if (cA == 0) cA = c;
				else if ((cB == 0)&&(cA != c)) cB = c;
			}
			++j;
		}

		if ((cA != 0) && (cB != 0))
		{
			uint rA0 = ((unsigned char*)&cA)[2] - ((unsigned char*)&color0)[2];
			uint gA0 = ((unsigned char*)&cA)[1] - ((unsigned char*)&color0)[1];
			uint bA0 = ((unsigned char*)&cA)[0] - ((unsigned char*)&color0)[0];

			uint rB1 = ((unsigned char*)&cB)[2] - ((unsigned char*)&color1)[2];
			uint gB1 = ((unsigned char*)&cB)[1] - ((unsigned char*)&color1)[1];
			uint bB1 = ((unsigned char*)&cB)[0] - ((unsigned char*)&color1)[0];

			uint rA1 = ((unsigned char*)&cA)[2] - ((unsigned char*)&color1)[2];
			uint gA1 = ((unsigned char*)&cA)[1] - ((unsigned char*)&color1)[1];
			uint bA1 = ((unsigned char*)&cA)[0] - ((unsigned char*)&color1)[0];

			uint rB0 = ((unsigned char*)&cB)[2] - ((unsigned char*)&color0)[2];
			uint gB0 = ((unsigned char*)&cB)[1] - ((unsigned char*)&color0)[1];
			uint bB0 = ((unsigned char*)&cB)[0] - ((unsigned char*)&color0)[0];



			uint distA0B1 = rA0 * rA0 + bA0 * bA0 + gA0 * gA0 + rB1 * rB1 + bB1 * bB1 + gB1 * gB1;
			uint distA1B0 = rB0 * rB0 + bB0 * bB0 + gB0 * gB0 + rA1 * rA1 + bA1 * bA1 + gA1 * gA1;

			if (distA0B1 < distA1B0)
			{
				for (int i = 0; i < w * h; ++i)
				{
					if (img[i] == cB) img[i] = 1;
					else img[i] = 0;
				}
			}
			else
			{
				for (int i = 0; i < w * h; ++i)
				{
					if (img[i] == cA) img[i] = 1;
					else img[i] = 0;
				}
			}
		}
		else
		{
			for (int i = 0; i < w * h; ++i)
			{
				uint c = img[i];
				uint a = (c >> 24) % 256;
				if (a == 0xFF) img[i] = 1;
				else img[i] = 0;
			}
		}
	}
}




void binarize2(MyImage im, bool inverse) {
	int w = im.w;
	int h = im.h;
	uint* img = im.img;
	if (img) {
		int* histogram = new int[256];
		for(int i=0; i<256; ++i) histogram[i] = 1;
		for(int i = 0; i < w * h; ++i) 
		{
			uint c = img[i];
			uint a = ((unsigned char*)&c)[3];
			uint r = ((unsigned char*)&c)[2];
			uint g = ((unsigned char*)&c)[1];
			uint b = ((unsigned char*)&c)[0];
			if (a == 0xFF) histogram[(r+g+b)/3]++;		
		}
		
		int cA = 64;
		int cB = 196;
		
		for (int j = 0; j < 10; ++j) 
		{
			int center = (cA + cB) / 2;
			int halfA = 0;
			for(int i=0; i < center; ++i) 
			{
				halfA += histogram[i];
			}
			halfA /= 2;
			cA = 0;
			int sumA = 0;
			while (sumA < halfA) 
			{
				sumA += histogram[cA];
				++cA;
			}
			--cA;
			
			int halfB = 0;
			for(int i=center; i < 256; ++i) 
			{
				halfB += histogram[i];
			}
			halfB /= 2;
			cB = center;
			int sumB = 0;
			while (sumB < halfB) 
			{
				sumB += histogram[cB];
				++cB;
			}
			--cB;
		}


		delete[] histogram;
		
		int median = (cA + cB) / 2;

		uint aa = 0; 
		uint bb = 1;
		if (inverse) {
			aa = 1; 
			bb = 0;
		}
		
		for(int i = 0; i < w * h; ++i) 
		{
			uint c = img[i];			
			
			uint a = ((unsigned char*)&c)[3];
			uint r = ((unsigned char*)&c)[2];
			uint g = ((unsigned char*)&c)[1];
			uint b = ((unsigned char*)&c)[0];
			if (a == 0xFF)
			{
				if ((r+g+b)/3 < (uint)median) img[i] = aa;
				else img[i] = bb;
			}
			else img[i] = bb;
			
		}	
	}
}


void remove_outside(MyImage im, MyContourPoint* contour, uint clearColor)
{
	uint* img = im.img;
	int w = im.w;
	int h = im.h;		
	if (img && contour) {
		int minX = w;
		int minY = h;
		int maxX = 0;
		int maxY = 0;

		MyContourPoint* currentContourPoint = contour;
		while (currentContourPoint->next) {
			MyPoint p = currentContourPoint->pos;
			if (p.X > maxX) maxX = p.X;
			if (p.X < minX) minX = p.X;
			if (p.Y > maxY) maxY = p.Y;
			if (p.Y < minY) minY = p.Y;
			currentContourPoint = currentContourPoint->next;
		}

		if (minX < 2) minX = 2;
		if (maxX > w-3) maxX = w-3;
		if (minY < 2) minY = 2;
		if (maxY > h-3) maxY = h-3;

		uint* ret = new uint[w * h];
		for (int i = 0; i < w * h; ++i) ret[i] = 0;          
		for (int y = minY-2; y < maxY+2; ++y)
		{	
			for (int x = minX-2; x < maxX+2; ++x)
			{
				ret[x + y * w] = 1; 
			}
		}	

		currentContourPoint = contour;
		while (currentContourPoint->next) {
			MyPoint p = currentContourPoint->pos;
			ret[p.X + p.Y * w] = 0;                
			currentContourPoint = currentContourPoint->next;
		}		

		MyImage im2 = {ret,w,h};
		clear_blob(im2 ,MyPoint(minX-1,minY-1));            

		for (int i = 0; i < w * h; ++i)
		{
			if (!ret[i]) img[i] = clearColor;                
		} 

		delete[] ret;
	}
}



void draw_line(MyImage im, MyPoint p1, MyPoint p2, uint color) {
	uint* img = im.img;
	int w = im.w;
	int h = im.h;		
	if (img) {
		int dx = p2.X - p1.X;
		int dy = p2.Y - p1.Y;
		int adx = abs(dx);
		int ady = abs(dy);
		int ddx;
		if (adx != 0) ddx = dx / adx;
		else ddx = 0;
		int ddy;
		if (ady != 0) ddy = dy / ady;		
		else ddy = 0;

		//if ((x >= 0)&&(x < im.w)&&(y >= 0)&&(y < im.h))
		if (adx > ady) {		
			if (adx == 0) adx = 1;
			for(int i=0; i <= adx; ++i)
			{
				img[p1.X + ddx * i  +  (p1.Y + (i * dy) / adx) * w] = color;
			}
		}
		else {
			if (ady == 0) ady = 1;
			for(int i=0; i <= ady; ++i)
			{
				img[p1.X + (i * dx) / ady  +  (p1.Y + ddy * i) * w] = color;
			}
		}
	}
}
void draw_shape_lines(MyImage im, MyContourPoint* contour, MyPoint pos, uint color) {
	uint* img = im.img;
	int w = im.w;
	int h = im.h;

	if (img && contour) {
		MyContourPoint* currentContourPoint = contour;
		MyPoint last = currentContourPoint->pos;
		last.X += pos.X;
		last.Y += pos.Y;
		while (currentContourPoint->next) {
			MyPoint p = currentContourPoint->pos;
			p.X += pos.X;
			p.Y += pos.Y;
			draw_line(im,p,last,color);
			last = p;	
			currentContourPoint = currentContourPoint->next;
		}
		MyPoint p = contour->pos;
		p.X += pos.X;
		p.Y += pos.Y;
		draw_line(im,p,last,color);
	}
}
void draw_shape(MyImage im, MyContourPoint* contour, MyPoint pos, uint color) {
	uint* img = im.img;
	int w = im.w;
	int h = im.h;

	if (img && contour) {
		MyContourPoint* currentContourPoint = contour;
		while (currentContourPoint->next) {
			MyPoint p = currentContourPoint->pos;
			int x = pos.X + p.X; 
			int y = pos.Y + p.Y;
			if ((x >= 0)&&(x < im.w)&&(y >= 0)&&(y < im.h))	img[x + y * w] = color;                
			currentContourPoint = currentContourPoint->next;
		}
	}
}

void draw_shape_list(MyImage im, MyShapeListElement* shList) {
	if (shList) {
		MyShapeListElement* currentShapeList = shList;
		while (currentShapeList->next) {
			draw_shape(im, currentShapeList->shape.contour,currentShapeList->shape.pos,1);             
			currentShapeList = currentShapeList->next;
		}
	}
}


void real_contour_N(MyImage im, MyShapeListElement** shapeList)
{
	uint* img = im.img;
	int w = im.w;
	int h = im.h;
	if (img && shapeList) 
	{
		vector<vector<MyPoint>> contours;
		for (int x = 1; x < w - 1; ++x)
		{
			for (int y = 1; y < h - 1; ++y)
			{
				if (img[x + y * w] == 1)
				{
					vector<MyPoint> contour;
					int x2 = x;
					int y2 = y;                        
					int phase = 7;
					do
					{
						contour.push_back(MyPoint(x2, y2));
						img[x2 + y2 * w]++;                            
						int x20 = x2;
						int y20 = y2;
						int count = 0;
						int x22 = x2;
						int y22 = y2;                            
						phase = (phase + 5) % 8;
						do
						{                                
							switch (phase)
							{
							case 0:
								x22 = x20 + 1;
								y22 = y20 + 0;
								break;
							case 1:
								x22 = x20 + 1;
								y22 = y20 + 1;
								break;
							case 2:
								x22 = x20 + 0;
								y22 = y20 + 1;
								break;
							case 3:
								x22 = x20 + -1;
								y22 = y20 + 1;
								break;
							case 4:
								x22 = x20 + -1;
								y22 = y20 + 0;
								break;
							case 5:
								x22 = x20 + -1;
								y22 = y20 + -1;
								break;
							case 6:
								x22 = x20 + 0;
								y22 = y20 + -1;
								break;
							case 7:
								x22 = x20 + 1;
								y22 = y20 + -1;
								break;
							}
							count++;
							phase = (phase + 1) % 8;
							if (x22 < 0) x22 = 0;
							if (y22 < 0) y22 = 0;
							if (x22 >= w) x22 = w-1;
							if (y22 >= h) y22 = h-1;
						}
						while ((count < 8) && (img[x22 + y22 * w] == 0));
						if (count < 8)
						{
							x2 = x22;
							y2 = y22;
						}

					}
					while (((x2 != x) || (y2 != y)) && (img[x2 + y2 * w] < 3));
					contours.push_back(contour);
					MyImage im2;
					im2.img = img;
					im2.w = w;
					im2.h = h;
					clear_blob(im2 ,MyPoint(x2,y2));					
				}
			}
		}

		MyShapeListElement* shList = (*shapeList) = new MyShapeListElement();

		for( vector<vector<MyPoint>>::iterator itcontour=contours.begin(); itcontour!=contours.end(); ++itcontour )
		{
			MyContourPoint* firstContourPoint;
			MyContourPoint* contourPoint = firstContourPoint = new MyContourPoint();
			
			for( vector<MyPoint>::iterator it=(*itcontour).begin(); it!=(*itcontour).end(); ++it )
			{								
				contourPoint->pos = *it;
				contourPoint->next = new MyContourPoint();
				contourPoint = contourPoint->next;						
			}		
			
			contourPoint->next = NULL;

			shList->shape.contour = firstContourPoint;
			shList->next = new MyShapeListElement();
			shList = shList->next;		
			
		}		
		
		shList->next = NULL;
		draw_shape_list(im,*shapeList);
		
	}
}	

void to_complex_N(MyContourPoint* contour, MyComplexData* out)
{
	if (contour && out) {
		int length = 0;
		MyContourPoint* cp = contour;
		while(cp->next) {
			++length;
			cp = cp->next;
		}


		double* ret = new double[length * 2];
		cp = contour;
		for (int i = 0; i < length; ++i)
		{

			ret[2 * i] = cp->pos.X;
			ret[2 * i + 1] = cp->pos.Y;
			cp = cp->next;
		}

		out->data = ret;
		out->length = length;		
	}
}

void to_contour_N(MyComplexData complexData, MyContourPoint** contour) 
{	
	if (contour)
	{		
		MyContourPoint* contourPoint = (*contour) = new MyContourPoint();

		for( int i = 0; i < complexData.length; ++i )
		{								
			contourPoint->pos = MyPoint((int)complexData.data[i*2],(int)complexData.data[i*2 + 1]);
			contourPoint->next = new MyContourPoint();
			contourPoint = contourPoint->next;			
		}		
		contourPoint->next = NULL;
	}
}

void resample_N(MyComplexData src, int destCount, MyComplexData* dest)
{
	if (dest) {
		double* ret = new double[destCount * 2];

		for (int i = 0; i < destCount; ++i)
		{
			double j = (double)i / destCount * src.length;

			int k = (int)floor(j);
			int l = (int)ceil(j);
			if (k == l)
			{
				ret[2 * i] = src.data[2 * k];
				ret[2 * i + 1] = src.data[2 * k + 1];
			}
			else
			{
				ret[2 * i] = src.data[(2 * k) % (2 * src.length)] * (1 - (j - floor(j))) + src.data[(2 * l) % (2 * src.length)] * (1 - (ceil(j) - j));
				ret[2 * i + 1] = src.data[(2 * k + 1) % (2 * src.length)] * (1 - (j - floor(j))) + src.data[(2 * l + 1) % (2 * src.length)] * (1 - (ceil(j) - j));
			}
		}

		dest->data = ret;
		dest->length = destCount;		
	}
}






void FFT(MyComplexData indata, int sign)
{
	uint n, mmax, m, j, istep, i;
	double wtemp, wr, wpr, wpi, wi, theta, tempr, tempi;

	double* data = indata.data;
	if (data) {
		int sampleCount = indata.length;


		n = ((uint)sampleCount) << 1;
		j = 0;
		for (i = 0; i < n / 2; i += 2)
		{
			if (j > i)
			{
				tempr = data[j];
				data[j] = data[i];
				data[i] = tempr;

				tempr = data[j + 1];
				data[j + 1] = data[i + 1];
				data[i + 1] = tempr;


				if ((j / 2) < (n / 4))
				{
					tempr = data[(n - (i + 2))];
					data[(n - (i + 2))] = data[(n - (j + 2))];
					data[(n - (j + 2))] = tempr;

					tempr = data[(n - (i + 2)) + 1];
					data[(n - (i + 2)) + 1] = data[(n - (j + 2)) + 1];
					data[(n - (j + 2)) + 1] = tempr;
				}
			}
			m = n >> 1;
			while (m >= 2 && j >= m)
			{
				j -= m;
				m >>= 1;
			}
			j += m;
		}
		//end of the bit-reversed order algorithm

		//Danielson-Lanzcos routine
		mmax = 2;
		while (n > mmax)
		{
			istep = mmax << 1;
			theta = sign * (2 * PI / mmax);
			wtemp = sin(0.5 * theta);
			wpr = -2.0 * wtemp * wtemp;
			wpi = sin(theta);
			wr = 1.0;
			wi = 0.0;
			for (m = 1; m < mmax; m += 2)
			{
				for (i = m; i <= n; i += istep)
				{
					j = i + mmax;
					tempr = wr * data[j - 1] - wi * data[j];
					tempi = wr * data[j] + wi * data[j - 1];
					data[j - 1] = data[i - 1] - tempr;
					data[j] = data[i] - tempi;
					data[i - 1] += tempr;
					data[i] += tempi;
				}
				wr = (wtemp = wr) * wpr - wi * wpi + wr;
				wi = wi * wpr + wtemp * wpi + wi;
			}
			mmax = istep;
		}

		if (sign < 0)
		{
			for (int i2 = 0; i2 < 2 * sampleCount; ++i2)
			{
				data[i2] *= (1.0 / sampleCount);
			}
		}
		//end of the algorithm
	}
}




//egy objektum normalizalasa; origoba tolas, meret normalizalas, fotengelyt fuggoleges allasba forditani (180 fokos elforgatas lehet!)
void normalize_shape(MyShape* shape)
{
	if (shape) {
		int sampleCount = SAMPLE_COUNT;

		MyComplexData complexData, complexData2;
		to_complex_N(shape->contour, &complexData);

		//toroljuk a shape contourjat, mert ujat fog kapni
		delete_contour_D(shape->contour);		

		resample_N(complexData, sampleCount, &complexData2);
		//toroljuk a regi complex datat
		delete[] complexData.data;

		complexData = complexData2;

		FFT(complexData, -1);
		double* data = complexData.data;

		//eltolas
		shape->pos.X += (int)data[0];
		shape->pos.Y += (int)data[1];

		//invariancia eltolasra
		data[0] = 0;
		data[1] = 0;


		//nagyitas
		double scalearea = sqrt(data[2] * data[2] + data[3] * data[3]) / SCALE_MULT;

		shape->scale *= scalearea;

		//invariancia nagyitasra
		for (int i2 = 0; i2 < sampleCount; ++i2)
		{
			data[2 * i2] *= 1 / scalearea;
			data[2 * i2 + 1] *= 1 / scalearea;
		}


		//elforgatas
		double ang1, ang2;
		ang1 = atan2(data[2], data[3]);
		ang2 = atan2(data[2 * sampleCount - 2], data[2 * sampleCount - 1]);
		double fi1 = (ang1 + ang2) / 2;
		double rotate = fi1;
		double fi2 = (ang1 - ang2) / 2;
		int n = (int)(fi2 * sampleCount / 2 / PI);
		fi2 = n * 2 * PI / sampleCount;

		shape->rot += fi1;
		if (shape->rot > PI) shape->rot -= PI;
		if (shape->rot < -PI) shape->rot += PI;

		//invariancia elforgatasra
		for (int i2 = 0; i2 < sampleCount; ++i2)
		{
			double angx = atan2(data[2 * i2], data[2 * i2 + 1]);
			double c = sqrt(data[2 * i2] * data[2 * i2] + data[2 * i2 + 1] * data[2 * i2 + 1]);
			data[2 * i2] = c * sin(angx - fi1 - i2 * fi2);
			data[2 * i2 + 1] = c * cos(angx - fi1 - i2 * fi2);
		}

		//visszatrafo es kirajzolas
		FFT(complexData, 1);

		to_contour_N(complexData,&shape->contour);

		delete[] complexData.data;
	}
}


//a marker kepekbol normalizalt marker objektumokat keszit
void generate_marker_shapes_N(MyImageListElement* markers) 
{
	
	if (markers) {
		MyImageListElement* currentMarker = markers;
		MyShapeListElement* currentShape = globalMarkerShapeList = new MyShapeListElement();
		
		int index = 0;
		while (currentMarker->next)
		{
			MyImage im;						
			clone_image_N(currentMarker->image, &im);	
			
			//segment_kmeans(im, 2);
			//binarize(im,0xFFFFFFFF,0xFF000000);
			binarize2(im,true);
			
			MyShapeListElement* markerContours;
			real_contour_N(im, &markerContours);
			
			MyShapeListElement* best = NULL;
			int max = 0;

			MyShapeListElement* currentMarkerShape = markerContours;
			while (currentMarkerShape->next)
			{
				int length = 0;
				MyContourPoint* cp = currentMarkerShape->shape.contour;
				while(cp->next) {
					++length;
					cp = cp->next;
				}

				if (length > max)
				{
					max = length;
					best = currentMarkerShape;
				}
				currentMarkerShape = currentMarkerShape->next;
			}

			MyShape bestShape;
			clone_shape_N(best->shape,&bestShape);
			delete_shape_list_D(markerContours);
				
			normalize_shape(&bestShape);			
			bestShape.index = index;
			bestShape.pos.X -= (im.w / 2);//kep kozepe az origo
			bestShape.pos.Y -= (im.h / 2);
			index++;

			currentShape->shape = bestShape;		

			currentShape->next = new MyShapeListElement();
			delete_image_D(im);
			currentMarker = currentMarker->next;
			currentShape = currentShape->next;
		}
		
		currentShape->next = NULL;
	}
}

//normalizalt markerek es normalizalt alakzat, megkeresi a megfelelo markert. ha tureshataron kivul van mind, akkor -1 lesz az index
void match_shape(MyShape* shape, MyShapeListElement* markers) {
	
	if (shape && markers) {
		MyShape rotatedShape;
		clone_shape_N(*shape,&rotatedShape);
		
		int sampleCount = SAMPLE_COUNT;
		MyComplexData complexData, complexData2;
		to_complex_N(rotatedShape.contour, &complexData);
		delete_contour_D(rotatedShape.contour);		
		resample_N(complexData, sampleCount, &complexData2);
		delete[] complexData.data;
		complexData = complexData2;
		FFT(complexData, -1);
		double* data = complexData.data;

		for (int i2 = 0; i2 < sampleCount; ++i2)
		{
			double angx = atan2(data[2 * i2], data[2 * i2 + 1]);
			double c = sqrt(data[2 * i2] * data[2 * i2] + data[2 * i2 + 1] * data[2 * i2 + 1]);
			data[2 * i2] = c * sin(angx - PI - i2 * PI);
			data[2 * i2 + 1] = c * cos(angx - PI - i2 * PI);
		}
		
		FFT(complexData, 1);
		to_contour_N(complexData,&rotatedShape.contour);
		delete[] complexData.data;

		rotatedShape.rot += PI;
		if (rotatedShape.rot > PI) rotatedShape.rot -= 2*PI;

		MyShapeListElement* current = markers;
		MyShapeListElement* best = NULL;
		double maxdif = 256*256*256*64;
		bool bestRotated = false;

		while(current->next) {
			MyContourPoint* currentCPshape = shape->contour;
			MyContourPoint* currentCPshapeRot = rotatedShape.contour;
			MyContourPoint* currentCPmarker = current->shape.contour;

			double dist = 0;
			double distRot = 0;
			while(currentCPmarker->next && currentCPshapeRot->next && currentCPshape->next) {
				dist += (currentCPmarker->pos.X - currentCPshape->pos.X) * (currentCPmarker->pos.X - currentCPshape->pos.X);
				dist += (currentCPmarker->pos.Y - currentCPshape->pos.Y) * (currentCPmarker->pos.Y - currentCPshape->pos.Y);

				distRot += (currentCPmarker->pos.X - currentCPshapeRot->pos.X) * (currentCPmarker->pos.X - currentCPshapeRot->pos.X);
				distRot += (currentCPmarker->pos.Y - currentCPshapeRot->pos.Y) * (currentCPmarker->pos.Y - currentCPshapeRot->pos.Y);

				currentCPmarker = currentCPmarker->next;
				currentCPshapeRot = currentCPshapeRot->next;
				currentCPshape = currentCPshape->next;
			}

			if (maxdif > dist) {
				maxdif = dist;
				best = current;
				bestRotated = false;
			}

			if (maxdif > distRot) {
				maxdif = distRot;
				best = current;
				bestRotated = true;
			}

			current = current->next;
		}

		


		if (bestRotated) {
			delete_contour_D(shape->contour);
			clone_shape_N(rotatedShape,shape);
			delete_contour_D(rotatedShape.contour);
		}
		else {
			delete_contour_D(rotatedShape.contour);
		}
		if (maxdif < TRESHOLD2) {
			shape->index = best->shape.index;
			shape->rot -= best->shape.rot;
			if (shape->rot < -PI) shape->rot += 2*PI;
			if (shape->rot > PI) shape->rot -= 2*PI;
			
			shape->scale /= best->shape.scale;		

			shape->pos.X -= (int)((best->shape.pos.X * cos(-shape->rot) - best->shape.pos.Y * sin(-shape->rot)) * shape->scale);
			shape->pos.Y -= (int)((best->shape.pos.X * sin(-shape->rot) + best->shape.pos.Y * cos(-shape->rot)) * shape->scale);



			to_complex_N(shape->contour, &complexData);
			delete_contour_D(shape->contour);				
			FFT(complexData, -1);
			data = complexData.data;

			
			int rotangn = (int)(-best->shape.rot * complexData.length / 2 / PI);
			double rotang = rotangn * 2 * PI / complexData.length;

			for (int i2 = 0; i2 < sampleCount; ++i2)
			{
				double angx = atan2(data[2 * i2], data[2 * i2 + 1]);
				double c = sqrt(data[2 * i2] * data[2 * i2] + data[2 * i2 + 1] * data[2 * i2 + 1]);
				data[2 * i2] = c * sin(angx - rotang - i2 * rotang);
				data[2 * i2 + 1] = c * cos(angx - rotang - i2 * rotang);
			}
			
			FFT(complexData, 1);
			to_contour_N(complexData,&(shape->contour));
			delete[] complexData.data;
		}
		else {
			shape->index = -1;
		}

		
	}
}
//megkeresi a megfeleleot, de nem konturt, hanem teruletet hasonlit ossze, kb 30-szor lassabb
void match_shape2(MyShape* shape, MyShapeListElement* markers) {
	
	if (shape && markers) {
		MyShape rotatedShape;
		clone_shape_N(*shape,&rotatedShape);
		
		int sampleCount = SAMPLE_COUNT;
		MyComplexData complexData, complexData2;
		to_complex_N(rotatedShape.contour, &complexData);
		delete_contour_D(rotatedShape.contour);		
		resample_N(complexData, sampleCount, &complexData2);
		delete[] complexData.data;
		complexData = complexData2;
		FFT(complexData, -1);
		double* data = complexData.data;

		for (int i2 = 0; i2 < sampleCount; ++i2)
		{
			double angx = atan2(data[2 * i2], data[2 * i2 + 1]);
			double c = sqrt(data[2 * i2] * data[2 * i2] + data[2 * i2 + 1] * data[2 * i2 + 1]);
			data[2 * i2] = c * sin(angx - PI - i2 * PI);
			data[2 * i2 + 1] = c * cos(angx - PI - i2 * PI);
		}
		
		FFT(complexData, 1);
		to_contour_N(complexData,&rotatedShape.contour);
		delete[] complexData.data;

		rotatedShape.rot += PI;
		if (rotatedShape.rot > PI) rotatedShape.rot -= 2*PI;		

		MyImage imShape;
		imShape.h = 100;
		imShape.w = 100;
		imShape.img = new uint[imShape.h * imShape.w];
		
		MyImage imShapeRot;
		imShapeRot.h = imShape.h;
		imShapeRot.w = imShape.w;
		imShapeRot.img = new uint[imShapeRot.h * imShapeRot.w];

		for(int i=0; i < imShape.h*imShape.w; ++i) {
			imShape.img[i] = 1;
			imShapeRot.img[i] = 1;
		}

		
		draw_shape_lines(imShape,shape->contour,MyPoint(imShape.w / 2,imShape.h / 2),0);
		draw_shape_lines(imShapeRot,rotatedShape.contour,MyPoint(imShapeRot.w / 2,imShapeRot.h / 2),0);

		clear_blob(imShape,MyPoint(0,0));
		clear_blob(imShape,MyPoint(imShape.w-1,0));
		clear_blob(imShapeRot,MyPoint(0,0));
		clear_blob(imShapeRot,MyPoint(imShapeRot.w-1,0));
		
		MyShapeListElement* current = markers;
		MyShapeListElement* best = NULL;
		double maxdif = 256*256*256*64;
		bool bestRotated = false;
		

		while(current->next) {					
			MyImage imMarker;
			imMarker.h = imShape.h;
			imMarker.w = imShape.w;
			imMarker.img = new uint[imMarker.h * imMarker.w];
			for(int i=0; i < imMarker.h*imMarker.w; ++i) {
				imMarker.img[i] = 1;				
			}
			draw_shape_lines(imMarker,current->shape.contour,MyPoint(imMarker.w / 2,imMarker.h / 2),0);
			clear_blob(imMarker,MyPoint(0,0));
			clear_blob(imMarker,MyPoint(imMarker.w-1,0));
	
			double dist = 0;
			double distRot = 0;

			for(int i=0; i < imMarker.h*imMarker.w; ++i) {
				if (imMarker.img[i] != imShape.img[i]) ++dist;
				if (imMarker.img[i] != imShapeRot.img[i]) ++distRot;
			}
		
			delete_image_D(imMarker);

			if (maxdif > dist) {
				maxdif = dist;
				best = current;
				bestRotated = false;
			}

			if (maxdif > distRot) {
				maxdif = distRot;
				best = current;
				bestRotated = true;
			}

			current = current->next;

		}

		delete_image_D(imShape);
		delete_image_D(imShapeRot);

		if (bestRotated) {
			delete_contour_D(shape->contour);
			clone_shape_N(rotatedShape,shape);
			delete_contour_D(rotatedShape.contour);
		}
		else {
			delete_contour_D(rotatedShape.contour);
		}
		shape->index = best->shape.index;
		shape->rot -= best->shape.rot;
		if (shape->rot < -PI) shape->rot += 2*PI;
		if (shape->rot > PI) shape->rot -= 2*PI;
		
		shape->scale /= best->shape.scale;		

		shape->pos.X -= (int)((best->shape.pos.X * cos(-shape->rot) - best->shape.pos.Y * sin(-shape->rot)) * shape->scale);
		shape->pos.Y -= (int)((best->shape.pos.X * sin(-shape->rot) + best->shape.pos.Y * cos(-shape->rot)) * shape->scale);



		to_complex_N(shape->contour, &complexData);
		delete_contour_D(shape->contour);				
		FFT(complexData, -1);
		data = complexData.data;

		
		int rotangn = (int)(-best->shape.rot * complexData.length / 2 / PI);
		double rotang = rotangn * 2 * PI / complexData.length;

		for (int i2 = 0; i2 < sampleCount; ++i2)
		{
			double angx = atan2(data[2 * i2], data[2 * i2 + 1]);
			double c = sqrt(data[2 * i2] * data[2 * i2] + data[2 * i2 + 1] * data[2 * i2 + 1]);
			data[2 * i2] = c * sin(angx - rotang - i2 * rotang);
			data[2 * i2 + 1] = c * cos(angx - rotang - i2 * rotang);
		}
		
		FFT(complexData, 1);
		to_contour_N(complexData,&(shape->contour));
		delete[] complexData.data;

		
	}
}
//elvegzi egy kepframere a teljes kepfeldolgozast, kamerakep, hatterkep, markerek listaja, es a visszakapott objektumlista)
void find_objects_N(MyImage foreground, MyImage background, MyShapeListElement** shapeList)
{
	MyImage foreground2;
	MyImage foregroundx;
	MyImage backgroundx;
	clone_image_N(foreground, &foregroundx);
	clone_image_N(background, &backgroundx);
	
	clone_image_N(foregroundx, &foreground2);
	
	segment_background_HSL(foregroundx, backgroundx, TRESHOLD1);
	closing(foregroundx);
	opening(foregroundx);

	MyShapeListElement* currentOutShapeElement;
	
	(*shapeList) = currentOutShapeElement = new MyShapeListElement();

	MyShapeListElement *currentShape;
	MyShapeListElement *firstShape;	
	real_contour_N(foregroundx, &firstShape);	

	currentShape = firstShape;	

	while (currentShape->next)
	{
	
		//kivagjuk a konturon belul levo teruletet
		MyImage foreground3;
		clone_image_N(foreground2, &foreground3);

		remove_outside(foreground3, currentShape->shape.contour, 0);
		//segment_kmeans(foreground3, 2);
		//binarize(foreground3,0xFFFFFFFF,0xFF000000);
		binarize2(foreground3,true);		

		MyShapeListElement* markerContours;
		real_contour_N(foreground3, &markerContours);

		MyShapeListElement* best = NULL;
		int max = 0;

		delete_image_D(foreground3);


		//maximalis hosszu kontur kivalasztasa (jo esetben csak 1 kontur van, a marker konturja)
		MyShapeListElement* currentMarkerShape = markerContours;
		while (currentMarkerShape->next)
		{
			int length = 0;
			MyContourPoint* cp = currentMarkerShape->shape.contour;
			while(cp->next) {
				++length;
				cp = cp->next;
			}

			if (length > max)
			{
				max = length;
				best = currentMarkerShape;
			}
			currentMarkerShape = currentMarkerShape->next;
		}
		
		MyShape bestShape;

		//ha megfelelo hosszu a kontur, csak akkor megyunk tovabb, amugy zajnak tekintjuk a markert, es a korulhatarolo kontur lesz amit megtalalunk
		if (best && (max > MIN_CONTOUR_LENGTH)) {		
			clone_shape_N(best->shape,&bestShape);
			normalize_shape(&bestShape);
			match_shape(&bestShape, globalMarkerShapeList);
		}
		else {			
			clone_shape_N(currentShape->shape,&bestShape);
			normalize_shape(&bestShape);
			bestShape.index = -1;
		}
						
		currentOutShapeElement->shape = bestShape;
		currentOutShapeElement->next = new MyShapeListElement();
		currentOutShapeElement = currentOutShapeElement->next;									

		delete_shape_list_D(markerContours);		

		//kovetkezo kepet vesszuk			
		currentShape = currentShape->next;			
	}	

	delete_shape_list_D(firstShape);
    
	currentOutShapeElement->next = NULL;		

	delete_image_D(foregroundx);
	delete_image_D(backgroundx);
	delete_image_D(foreground2);
		
}

MyImage readImage(char* filename, int w, int h) {
	MyImage im;
	im.w = w;
	im.h = h;
	im.img = new uint[im.w * im.h];
	
	ifstream is;
	is.open(filename, ios::binary );	
	is.read((char*)im.img,im.w * im.h * sizeof(uint));
	is.close();

	for(int i=0; i < im.w * im.h; ++i) {
		uint c = im.img[i];		
		unsigned char r = ((unsigned char*)&c)[0];
		unsigned char g = ((unsigned char*)&c)[1];
		unsigned char b = ((unsigned char*)&c)[2];
		unsigned char a = ((unsigned char*)&c)[3];
		c = im.img[i] = a*0x01000000 + b*0x00010000 + g*0x00000100 + r; 
	}

	return im;
}

void writeImage(char* filename, MyImage im) {
	ofstream os;
	os.open(filename, ofstream::binary);	
	os.write((char*)im.img,im.w * im.h * sizeof(uint));
	os.close();
}

void writeImageRGB(char* filename, MyImage im) {
	ofstream os;
	os.open(filename, ofstream::binary);	
	char* iii = (char*)im.img;
	for(int i=0; i < im.w * im.h; ++i) {
		os.write(iii, 3 * sizeof(char));
		iii += 4;
	}
	os.close();
}