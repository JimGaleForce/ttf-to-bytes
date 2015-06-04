#include "stdafx.h"
#include <iostream>

const int gridcolumns = 16;
const int gridrows = 12;

struct _Character {
    unsigned int offsetX, offsetY, width, height;
    bool dots[gridcolumns][gridrows];
};

struct _Character Grid;

std::string hex = "0123456789ABCDEF";

void doChar(char* pack, unsigned int length, struct _Character *chr) {

    chr->offsetX = hex.find(pack[1]);
    chr->offsetY = hex.find(pack[2]);
    chr->width = hex.find(pack[3]);
    chr->height = hex.find(pack[4]);

    unsigned int wordIndex = 0;
    unsigned int row = 0;
    unsigned int value = 0;
    unsigned int index = 0;
    unsigned int column = 0;
    unsigned int xcolumn = 0;
    for (unsigned int i = 5; i < length; i++)
    {
        unsigned int bitValue = hex.find(pack[i]);
        //Serial.print("...");
        //Serial.print(bitValue);
        for (unsigned int xrow = 0; xrow<4; xrow++) {
            //std::cout << bitValue << ',';
            chr->dots[column][row] = (bitValue & 0x0001) == 0x0001;
            //std::cout << (chr->dots[column][row] ? "X" : ".");
            bitValue = bitValue >> 1;
            //Serial.print(",");
            //Serial.print(bitValue);
            if (++row >= gridrows) {
                ++column;
                row = 0;
                break;
            }
        }

        std::cout << "\n";

    }
}

void shiftGrid()
{
    for (int grow = 0; grow < gridrows; grow++)
    {
        for (int gcol = 0; gcol < gridcolumns; gcol++)
        {
            Grid.dots[gcol][grow] = gcol == gridcolumns-1 ? false : Grid.dots[gcol + 1][grow];
        }
    }
}

void placeCharRowOnGrid(struct _Character *chr, int charColOffset)
{
    for (unsigned int row = 0; row < chr->height; row++) {
        Grid.dots[gridcolumns - 1][row] = chr->dots[charColOffset][row];
    }
}

void clearGrid()
{
    for (int grow = 0; grow < gridrows; grow++)
    {
        for (int gcol = 0; gcol < gridcolumns; gcol++)
        {
            Grid.dots[gcol][grow] = false;
        }
    }
}

void displayGrid()
{
    for (int grow = 0; grow < gridrows; grow++)
    {
        for (int gcol = 0; gcol < gridcolumns; gcol++)
        {
            std::cout << (Grid.dots[gcol][grow] ? "X" : ".");
        }

        std::cout << "\n";
    }
}

int _tmain(int argc, _TCHAR* argv[])
{
    struct _Character chr;

    doChar("!0389081060C50340340C50060081", 29, &chr);
    //doChar("!0389000000000000000000000000", 29, &chr);

    clearGrid();

    std::cout << "\n";
    for (int i = 0; i < chr.width; i++)
    {
        placeCharRowOnGrid(&chr, i);
        displayGrid();
        std::cout << "\n";
        shiftGrid();
    }

    displayGrid();

    char nothing;
    std::cin >> nothing;
	return 0;
}

