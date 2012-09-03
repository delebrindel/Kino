using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;

namespace Kino
{

    class Ardui
    {
        
    private SerialPort serialPort1;
    private SerialPort CrearPuerto(){
        serialPort1 = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
        serialPort1.DataReceived += 
          new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        return(serialPort1);
    }

    static void Main(string[] args)
    {
        SerialPort port = new SerialPort("COM3", 9600);
        port.Open();
        while (true)
        {
            String s = Console.ReadLine();
            if (s.Equals("exit"))
            {
                break;
            }
            port.Write(s + '\n');
        }
        port.Close();

    }


  // Receice and process data here.
  private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
  {
      // ...
  }

  // Send data.
  private void SendSerialData()
  {
      byte[] SerialData = new byte[4];
      serialPort1.Write(SerialData, 0, SerialData.Length);
  }
            static void Inicia_Arduino(string[] args)
            {
                SerialPort port = new SerialPort("COM1", 9600);
                port.Open();
                if (port.IsOpen)
                {
                    while (true)
                        Console.Write(port.ReadExisting());
                }
                port.Close();
            }


            public void Arduino_PortOpen(SerialPort puertoArd)
            {
                if (!puertoArd.IsOpen)
                {
                    puertoArd.Open();
                }
                else
                {
                    MessageBox.Show("Ya está abierto el puerto!!");
                }


                /* System.ComponentModel.IContainer components = new System.ComponentModel.Container();
                 if (!serialPort1.IsOpen)
                 {
                     Console.WriteLine("Oops");
                     return;
                 }
            
                 MessageBox.Show(serialPort1.IsOpen.ToString());

                 // this turns on !
                 serialPort1.DtrEnable = true;

                 // callback for text coming back from the arduino
                 serialPort1.Write("Mao");
                 serialPort1.Write("Texto");
                 // give it 2 secs to start up the sketch
                 System.Threading.Thread.Sleep(2000);*/

            }


            public void Arduino_PortClose(SerialPort puertoArd)
            {
                if (puertoArd.IsOpen)
                    puertoArd.Close();
                else
                    MessageBox.Show("El Puerto Está Cerrado!!");
            }

            public void Arduino_EnviarDatos(SerialPort puertoArd, string Texto)
            {
                puertoArd.Write(Texto); 
            }

        }
}
