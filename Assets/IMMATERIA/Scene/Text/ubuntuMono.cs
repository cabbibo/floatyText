﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ubuntuMono {

  public static float glyphWidth = 80.5f;
  public static float glyphHeight = 145f;
  public static float glyphBelow =   29f;

  public static Dictionary<int,float[]> info = new Dictionary<int,float[]>(){
      //ca, new float[]{   x      y    w     h     xoffset   yoffset   }}
    { 0   , new float[]{ 1016 , 972 , 4   , 4   , -1.500f  ,   1.500f  }}, 
    { 8   , new float[]{ 1016 , 968 , 4   , 4   , -1.500f  ,   1.500f  }}, 
    { 9   , new float[]{ 1016 , 964 , 4   , 4   , -1.500f  ,   1.500f  }}, 
    { 13  , new float[]{ 1016 , 960 , 4   , 4   , -1.500f  ,   1.500f  }}, 
    { 29  , new float[]{ 1016 , 956 , 4   , 4   , -1.500f  ,   1.500f  }}, 
    { 32  , new float[]{ 1016 , 952 , 4   , 4   , -1.500f  ,   1.500f  }}, 
    { 33  , new float[]{ 719  , 0   , 25  , 105 , 27.625f  , 101.188f  }}, 
    { 34  , new float[]{ 745  , 966 , 42  , 44  , 19.250f  , 110.875f  }}, 
    { 35  , new float[]{ 745  , 631 , 75  , 103 ,  2.875f  , 101.188f  }}, 
    { 36  , new float[]{ 271  , 0   , 67  , 131 ,  7.188f  , 113.125f  }}, 
    { 37  , new float[]{ 458  , 0   , 78  , 107 ,  1.375f  , 103.313f  }}, 
    { 38  , new float[]{ 609  , 421 , 75  , 107 ,  3.500f  , 103.500f  }}, 
    { 39  , new float[]{ 787  , 966 , 17  , 47  , 31.813f  , 110.875f  }}, 
    { 40  , new float[]{ 538  , 780 , 48  , 143 , 16.688f  , 114.438f  }}, 
    { 41  , new float[]{ 538  , 637 , 48  , 143 , 16.688f  , 114.438f  }}, 
    { 42  , new float[]{ 678  , 961 , 66  , 63  ,  7.688f  , 101.188f  }}, 
    { 43  , new float[]{ 734  , 107 , 69  , 74  ,  5.875f  ,  79.125f  }}, 
    { 44  , new float[]{ 771  , 27  , 36  , 49  , 22.438f  ,  23.875f  }}, 
    { 45  , new float[]{ 538  , 1005, 39  , 16  , 21.063f  ,  48.688f  }}, 
    { 46  , new float[]{ 842  , 990 , 27  , 28  , 27.000f  ,  24.188f  }}, 
    { 47  , new float[]{ 209  , 0   , 62  , 143 ,  9.563f  , 114.125f  }}, 
    { 48  , new float[]{ 609  , 751 , 69  , 108 ,  5.875f  , 103.500f  }}, 
    { 49  , new float[]{ 664  , 210 , 62  , 103 , 10.500f  , 101.188f  }}, 
    { 50  , new float[]{ 884  , 313 , 66  , 106 ,  7.688f  , 103.500f  }}, 
    { 51  , new float[]{ 751  , 313 , 65  , 108 ,  7.938f  , 103.500f  }}, 
    { 52  , new float[]{ 944  , 734 , 73  , 103 ,  4.313f  , 101.188f  }}, 
    { 53  , new float[]{ 458  , 726 , 64  , 105 ,  9.063f  , 101.188f  }}, 
    { 54  , new float[]{ 891  , 421 , 67  , 106 ,  7.188f  , 101.563f  }}, 
    { 55  , new float[]{ 538  , 534 , 67  , 103 ,  8.625f  , 101.250f  }}, 
    { 56  , new float[]{ 609  , 859 , 68  , 108 ,  6.688f  , 103.500f  }}, 
    { 57  , new float[]{ 678  , 855 , 67  , 106 ,  6.688f  , 103.438f  }}, 
    { 58  , new float[]{ 744  , 0   , 27  , 80  , 27.000f  ,  76.188f  }}, 
    { 59  , new float[]{ 988  , 107 , 36  , 101 , 18.625f  ,  76.188f  }}, 
    { 60  , new float[]{ 877  , 955 , 69  , 68  ,  6.375f  ,  74.625f  }}, 
    { 61  , new float[]{ 609  , 967 , 69  , 46  ,  5.875f  ,  64.813f  }}, 
    { 62  , new float[]{ 947  , 952 , 69  , 68  ,  6.375f  ,  74.625f  }}, 
    { 63  , new float[]{ 536  , 0   , 55  , 107 , 13.313f  , 103.438f  }}, 
    { 64  , new float[]{ 75   , 0   , 72  , 127 ,  4.938f  , 103.438f  }}, 
    { 65  , new float[]{ 458  , 107 , 81  , 103 , -0.063f  , 101.188f  }}, 
    { 66  , new float[]{ 684  , 313 , 67  , 105 ,  7.188f  , 102.000f  }}, 
    { 67  , new float[]{ 877  , 847 , 70  , 108 ,  5.875f  , 103.500f  }}, 
    { 68  , new float[]{ 684  , 421 , 69  , 105 ,  7.188f  , 102.188f  }}, 
    { 69  , new float[]{ 538  , 210 , 63  , 103 , 13.125f  , 101.188f  }}, 
    { 70  , new float[]{ 963  , 631 , 60  , 103 , 13.125f  , 101.188f  }}, 
    { 71  , new float[]{ 609  , 643 , 69  , 108 ,  5.875f  , 103.500f  }}, 
    { 72  , new float[]{ 895  , 528 , 70  , 103 ,  5.750f  , 101.188f  }}, 
    { 73  , new float[]{ 965  , 528 , 55  , 103 , 12.813f  , 101.188f  }}, 
    { 74  , new float[]{ 458  , 831 , 62  , 105 ,  7.188f  , 101.188f  }}, 
    { 75  , new float[]{ 820  , 631 , 72  , 103 ,  8.813f  , 101.188f  }}, 
    { 76  , new float[]{ 601  , 210 , 63  , 103 , 13.125f  , 101.188f  }}, 
    { 77  , new float[]{ 825  , 528 , 70  , 103 ,  5.438f  , 101.188f  }}, 
    { 78  , new float[]{ 458  , 416 , 66  , 103 ,  7.375f  , 101.188f  }}, 
    { 79  , new float[]{ 609  , 313 , 75  , 108 ,  3.188f  , 103.500f  }}, 
    { 80  , new float[]{ 458  , 622 , 65  , 104 , 10.250f  , 102.000f  }}, 
    { 81  , new float[]{ 0    , 0   , 75  , 132 ,  3.188f  , 103.313f  }}, 
    { 82  , new float[]{ 822  , 421 , 69  , 104 ,  7.375f  , 102.000f  }}, 
    { 83  , new float[]{ 678  , 747 , 66  , 108 ,  7.313f  , 103.500f  }}, 
    { 84  , new float[]{ 892  , 631 , 71  , 103 ,  4.750f  , 101.188f  }}, 
    { 85  , new float[]{ 753  , 421 , 69  , 105 ,  6.250f  , 101.250f  }}, 
    { 86  , new float[]{ 458  , 313 , 80  , 103 ,  0.563f  , 101.188f  }}, 
    { 87  , new float[]{ 754  , 528 , 71  , 103 ,  5.438f  , 101.188f  }}, 
    { 88  , new float[]{ 678  , 528 , 76  , 103 ,  2.500f  , 101.188f  }}, 
    { 89  , new float[]{ 458  , 210 , 80  , 103 ,  0.563f  , 101.188f  }}, 
    { 90  , new float[]{ 816  , 313 , 68  , 103 ,  7.188f  , 101.188f  }}, 
    { 91  , new float[]{ 398  , 143 , 42  , 143 , 21.500f  , 114.125f  }}, 
    { 92  , new float[]{ 147  , 0   , 62  , 143 ,  9.750f  , 114.125f  }}, 
    { 93  , new float[]{ 398  , 286 , 42  , 143 , 17.438f  , 114.125f  }}, 
    { 94  , new float[]{ 648  , 0   , 71  , 58  ,  4.938f  , 101.188f  }}, 
    { 95  , new float[]{ 807  , 27  , 81  , 15  , -0.188f  , -13.813f  }}, 
    { 96  , new float[]{ 811  , 990 , 31  , 32  , 24.688f  , 113.125f  }}, 
    { 97  , new float[]{ 607  , 107 , 64  , 82  ,  7.813f  ,  78.000f  }}, 
    { 98  , new float[]{ 745  , 734 , 66  , 117 ,  9.938f  , 113.125f  }}, 
    { 99  , new float[]{ 539  , 107 , 68  , 82  ,  6.375f  ,  78.063f  }}, 
    { 100 , new float[]{ 811  , 734 , 66  , 117 ,  4.938f  , 113.125f  }}, 
    { 101 , new float[]{ 947  , 210 , 70  , 82  ,  4.938f  ,  78.000f  }}, 
    { 102 , new float[]{ 538  , 313 , 71  , 115 ,  8.625f  , 113.125f  }}, 
    { 103 , new float[]{ 958  , 421 , 66  , 107 ,  4.938f  ,  78.000f  }}, 
    { 104 , new float[]{ 745  , 851 , 62  , 115 ,  9.938f  , 113.125f  }}, 
    { 105 , new float[]{ 877  , 734 , 67  , 113 ,  7.188f  , 109.438f  }}, 
    { 106 , new float[]{ 811  , 851 , 54  , 139 ,  9.938f  , 109.375f  }}, 
    { 107 , new float[]{ 609  , 528 , 69  , 115 ,  9.938f  , 113.188f  }}, 
    { 108 , new float[]{ 678  , 631 , 67  , 116 ,  7.188f  , 112.313f  }}, 
    { 109 , new float[]{ 875  , 210 , 72  , 80  ,  5.125f  ,  77.813f  }}, 
    { 110 , new float[]{ 865  , 107 , 62  , 80  ,  9.938f  ,  77.875f  }}, 
    { 111 , new float[]{ 538  , 923 , 71  , 82  ,  4.938f  ,  78.063f  }}, 
    { 112 , new float[]{ 950  , 313 , 66  , 106 ,  9.938f  ,  77.875f  }}, 
    { 113 , new float[]{ 538  , 428 , 66  , 106 ,  4.938f  ,  77.875f  }}, 
    { 114 , new float[]{ 591  , 0   , 57  , 80  , 15.750f  ,  77.875f  }}, 
    { 115 , new float[]{ 671  , 107 , 63  , 82  ,  9.063f  ,  78.000f  }}, 
    { 116 , new float[]{ 458  , 519 , 66  , 103 ,  8.625f  ,  99.500f  }}, 
    { 117 , new float[]{ 803  , 107 , 62  , 80  ,  9.438f  ,  76.250f  }}, 
    { 118 , new float[]{ 801  , 210 , 74  , 78  ,  3.313f  ,  76.250f  }}, 
    { 119 , new float[]{ 458  , 936 , 78  , 78  ,  1.500f  ,  76.250f  }}, 
    { 120 , new float[]{ 726  , 210 , 75  , 78  ,  3.125f  ,  76.250f  }}, 
    { 121 , new float[]{ 947  , 847 , 72  , 105 ,  4.250f  ,  76.250f  }}, 
    { 122 , new float[]{ 927  , 107 , 61  , 78  , 10.250f  ,  76.250f  }}, 
    { 123 , new float[]{ 398  , 0   , 60  , 143 , 11.188f  , 114.125f  }}, 
    { 124 , new float[]{ 586  , 780 , 16  , 143 , 32.625f  , 114.063f  }}, 
    { 125 , new float[]{ 338  , 0   , 60  , 143 , 10.250f  , 114.125f  }}, 
    { 126 , new float[]{ 771  , 0   , 72  , 27  ,  4.438f  ,  55.813f  }} 
  };

}