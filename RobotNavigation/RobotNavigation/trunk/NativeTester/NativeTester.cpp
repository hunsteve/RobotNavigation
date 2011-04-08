// NativeTester.cpp : Defines the entry point for the console application.
//
#include "MLPDll.h"
#include <iostream>
#include <fstream>
#include <stdio.h>
#include <math.h>

using namespace std;

#define nTrainData 100

int main(int argc, char* argv[])
{	
	
	float x[nTrainData];
	float y[nTrainData];
	float yn[nTrainData];
	int i=0;
	for (float f=-1; (f<1)&&(i<nTrainData); f += 0.02) {		
		x[i] = f;
		y[i] = sin(f*10);		
		++i;
	}

	cout << "Training started!\r\n";

	int counts[2] = {50, 1};
	MLP mlp = createMLP(1,counts,2, false);
	Matrix input = createMatrix(1,1);
	Matrix error = createMatrix(1,1);

	for (int k=0; k<3000; ++k) {
		for (int j=0; j<nTrainData; ++j) {
			input.data[0] = x[j];
			Matrix output = Output(mlp,input);
			error.data[0] = y[j] - output.data[0];
			Train(mlp, error, 0.05);
		}
		if (k%100 == 0) cout << k << "\r\n";		
	}

	cout << "Training ended!\r\n";		

	for (int j=0; j<nTrainData; ++j) {
		input.data[0] = x[j];
		Matrix output = Output(mlp,input);
		cout << output.data[0] << "  " << y[j] << "\r\n";		
	}



//
//	/*MyImage cam = readImage("1.raw",20,20);
//	binarize2(cam,true);		
//	closing(cam);
//	opening(cam);
//	writeImage("1b.raw",cam);
//*/
//	MyImage cam = readImage("cam.raw",320,240);
//	MyImage back = readImage("back.raw",320,240);
//	
//	MyImage foreground2;
//	MyImage foregroundx;
//	MyImage backgroundx;
//	clone_image_N(cam, &foregroundx);
//	clone_image_N(back, &backgroundx);	
//	clone_image_N(cam, &foreground2);
//
//	segment_background(foregroundx,backgroundx,TRESHOLD1);
//
//	writeImage("segment_background.raw",foregroundx);
//
//	closing(foregroundx);
//	
//	writeImage("segment_background_closing.raw",foregroundx);
//
//	opening(foregroundx);
//	
//	writeImage("segment_background_opening_closing.raw",foregroundx);
//
//	MyShapeListElement* currentOutShapeElement;
//	
//	currentOutShapeElement = new MyShapeListElement();
//
//	MyShapeListElement *currentShape;
//	MyShapeListElement *firstShape;	
//
//	real_contour_N(foregroundx, &firstShape);	
//
//	writeImage("contour_objects.raw",foregroundx);
//
//
//	/*currentShape = firstShape;	
//	int count = 0;
//	while (currentShape->next)
//	{
//		MyImage im;
//		im.w = 320;
//		im.h = 240;
//		im.img = new uint[im.w * im.h];
//		draw_shape(im,currentShape->shape.contour,MyPoint(0,0),1);
//
//		char s[100];
//		sprintf(s,"shape_object_%d.raw",count);
//		count++;
//		writeImage(s,cam);
//
//		currentShape = currentShape->next;			
//	}	*/
//
//
//	currentShape = firstShape;	
//	int count = 0;
//	while (currentShape->next)
//	{
//	
//		//kivagjuk a konturon belul levo teruletet
//		MyImage foreground3;
//		clone_image_N(foreground2, &foreground3);
//
//		remove_outside(foreground3, currentShape->shape.contour, 0);
//		
//
//		char s[100];
//		sprintf(s,"only_object_%d.raw",count);		
//		writeImageRGB(s,foreground3);
//
//		
//		binarize2(foreground3,true);		
//
//		sprintf(s,"only_object_%d_binarized.raw",count);		
//		writeImage(s,foreground3);
//
//		MyShapeListElement* markerContours;
//		real_contour_N(foreground3, &markerContours);
//
//		sprintf(s,"only_object_%d_contour.raw",count);		
//		writeImage(s,foreground3);
//		count++;
//
//		MyShapeListElement* best = NULL;
//		int max = 0;
//
//		delete_image_D(foreground3);
//
//
//		//maximalis hosszu kontur kivalasztasa (jo esetben csak 1 kontur van, a marker konturja)
//		MyShapeListElement* currentMarkerShape = markerContours;
//		while (currentMarkerShape->next)
//		{
//			int length = 0;
//			MyContourPoint* cp = currentMarkerShape->shape.contour;
//			while(cp->next) {
//				++length;
//				cp = cp->next;
//			}
//
//			if (length > max)
//			{
//				max = length;
//				best = currentMarkerShape;
//			}
//			currentMarkerShape = currentMarkerShape->next;
//		}
//		
//		MyShape bestShape;
//
//		//ha megfelelo hosszu a kontur, csak akkor megyunk tovabb, amugy zajnak tekintjuk a markert, es a korulhatarolo kontur lesz amit megtalalunk
//		if (best && (max > MIN_CONTOUR_LENGTH)) {		
//			clone_shape_N(best->shape,&bestShape);
//			normalize_shape(&bestShape);
//			//match_shape(&bestShape, globalMarkerShapeList);
//		}
//		else {			
//			clone_shape_N(currentShape->shape,&bestShape);
//			normalize_shape(&bestShape);
//			bestShape.index = -1;
//		}
//						
//		currentOutShapeElement->shape = bestShape;
//		currentOutShapeElement->next = new MyShapeListElement();
//		currentOutShapeElement = currentOutShapeElement->next;									
//
//		delete_shape_list_D(markerContours);		
//
//		//kovetkezo kepet vesszuk			
//		currentShape = currentShape->next;			
//	}	
//
//
//	
//	
//	/*MyShapeListElement* shapeList = NULL;
//	real_contour_N(img2, &shapeList);       
//	if ((shapeList != NULL) && (shapeList->next != NULL))
//    {
//		normalize_shape(&(shapeList->shape));
//		cout<<"OK";
//	}
//
//	MyImageListElement* list;	
//	create_image_list_N(&list); 
//
//	MyImage im1 = readImage("marker1.raw",100,100);
//	add_image_list_item_N(&list, im1);
//
//	MyImage im2 = readImage("marker2.raw",100,100);
//	add_image_list_item_N(&list, im2);
//
//	MyImage im3 = readImage("marker3.raw",100,100);
//	add_image_list_item_N(&list, im3);
//
//	MyImage im4 = readImage("marker4.raw",100,100);
//	add_image_list_item_N(&list, im4);
//
//	MyImage im5 = readImage("marker5.raw",100,100);
//	add_image_list_item_N(&list, im5);
//	                  
//    generate_marker_shapes_N(list);
//    
//	delete_image_D(im1);
//	delete_image_D(im2);
//	delete_image_D(im3);
//	delete_image_D(im4);
//	delete_image_D(im5);
//
//    delete_image_list_D(list);
//
//    MyImage cam = readImage("cam.raw",320,240);
//	MyImage back = readImage("back.raw",320,240);
//
//	MyShapeListElement* retval;
//	MyShapeListElement* current;
//	find_objects_N(cam, back, &retval);
//	current = retval;
//	
//	while(current->next) {
//		printf("index: %d   x: %d   y: %d    rot: %.3f    scale: %.3f\r\n",current->shape.index,current->shape.pos.X, current->shape.pos.Y, current->shape.rot, current->shape.scale);
//		current = current->next;
//	}
//	
//	delete_shape_list_D(retval);
//
//	delete_image_D(cam);
//	delete_image_D(back);
//
//	*/
	int zzz;
	cin>>zzz;



	return 0;
}

