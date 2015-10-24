using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class DoubleQueue
{
    int pointer = 0;
    double[] list;
    double[] sorted;
    int size;
    int counter;

    public double Last { get { return list[(pointer) % size]; } }
    public double Previous { get { return list[( pointer - 1 + size) % size]; } }
    public double First { get { return list[0]; } }
    public DoubleQueue(int size)
    {
        list = new double[size];
        sorted = new double[list.Length];
        this.size = size;
    }
    public void Add(double item)
    {
        list[pointer] = item;
        pointer = (pointer + 1) % size;
        counter = counter >= size ? size : counter + 1;
    }

    public int Count { get { return counter; } }
    public double this[int i]
    {
        get
        {
            if (counter < size)
            {
                return list[i];
            }
            else
            {
                return list[(pointer + i) % size];
            }
        }
        set { list[(pointer + i) % size] = value; }
    }

    public void Clear()
    {
        pointer = 0;
        counter = 0;
    }
    public double Average()
    {
        double sum = 0;

        for (int i = 0; i < counter; i++)
        {
            sum += this[i];
        }

        return sum / counter;
    }
    public double Average(int min, int max)
    {
        max = Math.Min(max, counter);

        if (min < 0
            || max < 0
            || max <= min
            || counter == 0) return 0;

        double sum = 0;

        for (int i = min; i < max; i++)
        {
            sum += this[i];
        }

        return sum / (max - min);
    }
    public double[] ToArray()
    {
        for (int i = 0; i < list.Length; i++)
        {
            sorted[i] = this[i];
        }
        return sorted;
    }
    public double[] ToArray(int min, int max)
    {
        if (min < 0) throw new IndexOutOfRangeException("min value is less than zero");
        if (max > size) throw new IndexOutOfRangeException("max value is larger than the size of the array");
        if (max < min) throw new IndexOutOfRangeException("max value is less than min value");
        double[] temp = new double[max - min];
        int j = 0;
        for (int i = min; i < max; i++)
        {
            temp[j] = this[i];
            j++;
        }
        return temp;
    }
}

