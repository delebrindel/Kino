   M S B A M L   `   `   `  ����A  =KBasic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/$clr-namespace:KBasic;assembly=KBasicKBasic  5   	   D @KControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null8*clr-namespace:KControls;assembly=KControls	KControls 5   	    KWpf   KWpf.PaintWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   3my$clr-namespace:KBasic;assembly=KBasic     5   	   ;kino*clr-namespace:KControls;assembly=KControls     5   	   -   5          � Title$  Kinect Cursor Control��5   	   $	��700��6'   $
��1200��64    � WindowStartupLocation$ CenterScreen=�6n    � Cursor$
 Hand��5   9    � SizeToContent$ WidthAndHeight=�6G   .��5      �� .+�5   
   9� -   5        $��Expansor��5      {�.  .��5	         KBasic.SensorChooser  -   5        $��kinectSensorChooser��5	      $	��200��6:   5
      �� 5   
   -   5        $��video��5   '   $��80��6   $��60��6   6'   �� 5   
   -   5        $��VPai_LS_SelectedTool��5      $!��Herramienta Seleccionada:  ��60   $��28(�6S   �� 5   
   -   5        $��VPai_L_SelectedTool��5      $��Lápiz��6/   $��28(�6<   �� 5   
   $��30��6   .@�5      �� -   5        $��LeftHand��5   2   $��Center=�6   .��6B   
Modo Zurdo6N   �� 5      -   5        $��VPai_Ch_Seated��5   C   $��Center=�6    d� 	IsChecked$
 TrueY�62   .��6�   Modo Sentado6�   �� 5      -	   5        $��Status��5   4   $��Center=�6   $��-----��6B   $��24(�6O   $��50q�6]   �� 5      -
   5        $��Status2��5   3   $��Center=�6   $��------��6B   $��24(�6P   $	��100q�6^   �� 5      -   5        $��B_SubKinect��5      $��Subir Kinect��6(   6[   �� 5      -   5        $��B_BajKinect��5      $��Bajar Kinect��6(   6[   5      9� 5   
   -   5        $��	Expansor2��5      {�. .��5      �� .@�5        KControls.KHoverButton  -   5        $��VPai_HB_Nuevo��5   )      Time$ 1��6       DisplayTextSize$ 30Q�6>      DisplayText$ Nuevo��6S      PathFill
 �P��6x     5      -   5        $��VPai_HB_Guard��5   *   $ 1��6!   $ 30Q�6?   $ Guardar��6T   
 ����6�     5      -   5        $��VPai_HB_Lapi��5   *   $ 1��6!   $ 30Q�6>   $ Lápiz��6S   
 ��  �6w     5      -   5        $��VPai_HB_Pinc��5   *   $ 1��6!   $ 30Q�6>   $ Pincel��6S   
 �  ��6w     5      -   5        $��VPai_HB_Goma��5   *   $ 1��6!   $ 30Q�6>   $
 Goma��6S   
 � � �6w     5      -   5        $��VPai_HB_Borr��5   *   $ 1��6!   $ 30Q�6>   $ Borrar��6S   
 � ��6~     5      -   5        $��VPai_HB_Resa��5   *   $ 1��6!   $ 30Q�6>   $ Resalta��6S   
 �P��6z   5      5       �� 5!   
   -   5        $��Dibujo��5!      $	��600��6"   $
��1200��6/   	 J� Opacity$	 1Q�6<   $ Cross��6H   5"      5#      