   M S B A M L   `   `   `  ����  ChemCompLogger    ChemCompLogger.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  
MainWindow��5   	   $	��300��6   $	��400��6)   $	��300��5   	   $	��400��6   .��5	      � -   5        $��
WindowGrid��5
   	   ���#051A38 � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��1* �6   .+�5   
   � -   5        $��
MasterGrid��5      $��1��5       � ColumnDefinitions 5      �� 5      $��1* �6#   �� 5      $��1* �6#   .+�5!      � -   5         �Name$ 
ButtonGrid��5!      $��0��5"      $��8q�5#       K� ZIndex$ 8��5$       5%      �� 5&      $��1* �6'   �� 5'      $��1* �6'    5)      �� 5*      $��1* �6$   �� 5+      $��1* �6$   �� 5,      $��1* �6$   �� 5-      $��1* �6$   5/      �� 51      -   5        $��tableHeader��52      $��8,0,8,8q�53      $��0��54      $
��Left=�55      $y�Hidden=�56      
�������$��24(�58      59      �� 5;      -   5         ��Name$ textBoxGameStatus��5<      $
��Auto��5=      " �� VerticalScrollBarVisibility$ Hidden=�5>      $��1��5?       �� TextWrapping$
 Wrap=�5@       �� AcceptsReturn . 	 �� 
IsReadOnly	 . $��16(�5C      $��Consolas)�5D      ���#051A38���#A6B5CB$��0q�5G      
 �� MaxLines$	
 200��5H      .��6    5I      5J      5K      5L      