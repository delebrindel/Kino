   M S B A M L   `   `   `  ����S  OMicrosoft.Kinect.Toolkit, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nulleHclr-namespace:Microsoft.Kinect.Toolkit;assembly=Microsoft.Kinect.ToolkitMicrosoft.Kinect.Toolkit  5   	    KWpf   KWpf.ArduinoC   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   YmsktHclr-namespace:Microsoft.Kinect.Toolkit;assembly=Microsoft.Kinect.Toolkit     5   	   -   5          �Name$  Arduino��5   	    � Title$
 Kino��5      $	��300��6   $
��1000��6    � Topmost .  � WindowStartupLocation$ CenterScreen=�6j    � WindowStyle$ ThreeDBorderWindow=�6�    � SizeToContent$ WidthAndHeight=�6�    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush��%�   &	        &	 #     &	 F     �� 5   
   $ 	#ff6e6e6e��62   �� 5	   
   $ 	#ff52318f��64   �� 5
   
   $ 	#ff00BCF2��62   5      .��5      � -   5        $��
layoutGrid��5      $��	10 0 10 0q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   ]� $��0��6   $��0 0 0 20q�6!   .+�5      �� $��Right=�6   $��Bottom=�6/   #��[  6J   $��Segoe UI)�6x   $��18(�6�   
Alfa 0.36�   5      �� 5   
   -   5        $��
BotonesArd��5   T   $��1��6   $��Stretch=�6!   ���#808CFF$��Bottom=�6f   $��0 0 0 10q�6�   .@�5      �� -   5        $��VArd_B_OPort��5   �   $��Abrir Puerto Serial��6   $
��Auto��63   $��Right=�6A   $��Center=�6]   $��40 0 0 0q�6x   6�   �� 5      -   5        $��VArd_B_CPort��5   �   $��Cerrar Puerto Serial��6   $
��Auto��64   $��Right=�6B   $��Center=�6^   $��40 0 0 0q�6y   6�   �� 5      -   5        $��VArd_B_HLed��5   �   $��Encender Led��6   $
��Auto��6,   $��Right=�6:   $��Center=�6V   $��20 0 0 0q�6q   6�   �� 5      -   5        $��VArd_B_LLed��5   �   $��
Apagar Led��6   $
��Auto��6*   $
��Left=�68   $��Center=�6S   $��20 0 0 0q�6n   6�   �� 5      -   5        $��VArd_B_BLed��5   �   $��Destello De Led��6   $
��Auto��6/   $
��Left=�6=   $��Center=�6X   $��20 0 0 0q�6s   6�   �� 5      -	   5        $��VArd_B_FBLed��5   �   $��Destello Rápido��6   $
��Auto��6/   $
��Left=�6=   $��Center=�6X   $��20 0 0 0q�6s   6�   5      �� 5   
   -
   5        $��ControlesKin��5   ?   $��2��6   $��Stretch=�6!   ���#E6E8FF$��Bottom=�6h   .@�5      �� -   5        $��VArd_LS_Port��5      $��Center=�6,   $
��Left=�6G   $��40 0 0 0q�6b   Estado Del Puerto6�   �� 5       -   5        $��VArd_L_Port��5       $��Center=�6,   $
��Left=�6G   $��20 0 0 0q�6b   
--------6~   5!      5"      5#      