   M S B A M L   `   `   `  ����  ShipsModern    ShipsModern.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   b ^SharpVectors.Converters.Wpf, Version=1.8.4.0, Culture=neutral, PublicKeyToken=b532964b8548be770SVG&http://sharpvectors.codeplex.com/svgc/  5   	   -   5          � Title$  Arctic Ships��5   	    � WindowState$ 	Maximized=�5	   ?    � Icon$ /TitleIcon.png��6W   .��5
      �  � 	Resources 5   
      InvisibleBorder �� 
TargetType �� Property �� Value(  "SharpVectors.Converters.SvgViewbox  playIcon   AutoSize   OptimizePath	   TextAsGeometry
   Source (� Stretch  	draghIcon  plusIcon  	minusIcon��%�  &	        &	 r     &	 �     &	 k    &	 �    �� 5      " ��6,   .�5      ��  � ��
 ���� 6E   �� 5       � ��$ 0q�64   6@   5        5      $
 True��6.   $ False��6>   $
	 True��6S   $
 /Resources/icons/play.svg?�6i   $ Uniform=�6�   6�     5      $
 True��6/   $ False��6?   $
	 True��6T   $ 
 /Resources/icons/dragh.svg?�6j   $ Uniform=�6�   6�     5      $
 True��6.   $ False��6>   $
	 True��6S   $
 /Resources/icons/plus.svg?�6i   $ Uniform=�6�   6�     5      $
 True��6/   $ False��6?   $
	 True��6T   $ 
 /Resources/icons/minus.svg?�6j   $ Uniform=�6�   6�   5      .+�5   
   �� -   5        $��fieldImg��5      $
��1920��6    $
��1080��6-   6>   �� 5   
   -   5        $��modelCanvas��5      
������ 5      �� 5   
    �� CornerRadius$ 5��6   $��1q�6"   
�������._�5      �� -   5        $��	timeLabel��5      $	��Top=�6&   $��Center=�6>   $��50q�6[   $��30(�6g   $��Bahnschrift)�6u   ��6�   ��  �� Path$ 
TimeFormat� �� UpdateSourceTrigger$ PropertyChanged=�6�   5      �� 5   
   
�������$	��Top=�6$   $��Center=�6<   $��1q�6Y   
�������$ 5��6�   $��5q�6�   ._�5      �� $��5q�6   $	��400��6$   $��25��63    �� Orientation$ 
Horizontal=�6?   $��Center=�6X   $	��Top=�6u   .+�5      �� -   5        #��[ 5      $��25��6>   #��[  6I   6r   �� 5       -   5        #��[ 5       $��25��6=   #��[  6H   6q   �� 5!      -   5        #��[ 5!      $��25��6>   #��[  6I   6r   �� 5"      -   5        $��controlTimeSlider��5"       � Minimum$ 1Q�62    � Maximum$ 100000Q�6>   $	��200��6O    �� AutoToolTipPlacement$ BottomRight=�6~    �� AutoToolTipPrecision$ 0��6�   6�   5#      5$      5%      5'      