   M S B A M L   `   `   `  ����D  @KControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null8*clr-namespace:KControls;assembly=KControls	KControls  5   	    KWpf   KWpf.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @	kcontrols*clr-namespace:KControls;assembly=KControls     5   	   -   5          �Name$  VentanaPrincipal��5   	    � Title$
 Kino��5   	   $	��400��6   $	��700��6#    � WindowStartupLocation$ CenterScreen=�6/    � WindowStyle$ ThreeDBorderWindow=�6T    � SizeToContent$ WidthAndHeight=�6u    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush��%�   &	        &	 #     &	 F     �� 5   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5	   
   $ 	#ff00BCF2��62   5
      .��5      � -   5        $��
layoutGrid��5      $��	10 0 10 0q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��1��6   $��Center=�6"   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��Center=�62   .+�5         KControls.KHoverButton  -   5        	   DisplayTextSize$	 16Q�5   )   
   DisplayText$
 Paint��6>      Time$ 1��6R      PathFill
 �  ��6j     5      -   5        $	 16Q�5   )   $��	20 0 20 0q�6>   $
 Speech��6Q   $ 1��6f   
 � � �6�     5      -   5        $	 16Q�5   )   $��	20 0 20 0q�6>   $
 
MultiTouch��6Q   $ 1��6j   
 � ���6�     5      -   5        $	 16Q�5   )   $��	20 0 20 0q�6>   $
 
PP Control��6Q   $ 1��6j   
 ��  �6�   5      �� 5      $ 
Horizontal=�6   $��Center=�62   .+�5        -   5        $	 16Q�5   )   $��
20 20 20 0q�6>   $
 Piano��6R   $ 1��6f   
 � ���6�     5      -   5        $	 16Q�5   )   $��
20 20 20 0q�6>   $
 Move!��6R   $ 1��6f   
 �""��6�     5      -	   5        $	 16Q�5   )   $��
20 20 20 0q�6>   $
 Instruments!��6R   $ 1��6m   
 � ��6�   5      5      5      5       