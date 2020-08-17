using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steg
{
    class CryptSteganography
    {
        #region Преобразования
        private string startSymbol = "*"; // Обозначение зашифрованного файла

        private BitArray ByteToBit(byte src)
        {
            BitArray bitArray = new BitArray(8);
            bool st = false;
            for (int i = 0; i < 8; i++)
            {
                if ((src >> i & 1) == 1)
                {
                    st = true;
                }
                else st = false;
                bitArray[i] = st;
            }
            return bitArray;
        }

        private byte BitToByte(BitArray scr)
        {
            byte result = 0;
            for (byte index = 0, m = 1; index < 8; index++, m *= 2)
                result += scr.Get(index) ? m : (byte)0;
            return result;
        }
        #endregion

        /* Проверка на сокрытие в файле информации */
        public bool isEncryption(Bitmap scr)
        {
            byte[] rez = new byte[1];
            Color color = scr.GetPixel(0, 0);
            BitArray colorArray = ByteToBit(color.R); //получаем байт цвета и преобразуем в массив бит
            BitArray messageArray = ByteToBit(color.R); ;//инициализируем результирующий массив бит
            messageArray[0] = colorArray[0];
            messageArray[1] = colorArray[1];

            colorArray = ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
            messageArray[2] = colorArray[0];
            messageArray[3] = colorArray[1];
            messageArray[4] = colorArray[2];

            colorArray = ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
            messageArray[5] = colorArray[0];
            messageArray[6] = colorArray[1];
            messageArray[7] = colorArray[2];
            rez[0] = BitToByte(messageArray); //получаем байт символа, записанного в 1 пикселе
            string m = Encoding.GetEncoding(1251).GetString(rez);

            if (m == startSymbol)
            {
                return true;
            }
            else return false;
        }

        #region Вспомогательные_процедуры
        /* Запись числа количества символов текста */
        private void WriteCountText(int count, Bitmap src)
        {
            /* Записываем в второй пиксель разрядность длины сообщения */
            byte need = Convert.ToByte(count.ToString().Length.ToString());
            BitArray messageArray = ByteToBit(need);

            Color color = src.GetPixel(1, 0);
            BitArray colorArray = ByteToBit(color.R); //получаем байт цвета и преобразуем в массив бит
            colorArray[0] = messageArray[0];
            colorArray[1] = messageArray[1];
            byte nR = BitToByte(colorArray);

            colorArray = ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
            colorArray[0] = messageArray[2];
            colorArray[1] = messageArray[3];
            colorArray[2] = messageArray[4];
            byte nG = BitToByte(colorArray);

            colorArray = ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
            colorArray[0] = messageArray[5];
            colorArray[1] = messageArray[6];
            colorArray[2] = messageArray[7];
            byte nB = BitToByte(colorArray);

            Color nColor = Color.FromArgb(nR, nG, nB);
            src.SetPixel(1, 0, nColor);

            for (int i = 0; i < count.ToString().Length; i++)
            {
                messageArray = ByteToBit(byte.Parse(count.ToString()[i].ToString()));

                color = src.GetPixel(0, i + 1);
                colorArray = ByteToBit(color.R); //получаем байт цвета и преобразуем в массив бит
                colorArray[0] = messageArray[0];
                colorArray[1] = messageArray[1];
                nR = BitToByte(colorArray);

                colorArray = ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
                colorArray[0] = messageArray[2];
                colorArray[1] = messageArray[3];
                colorArray[2] = messageArray[4];
                nG = BitToByte(colorArray);

                colorArray = ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
                colorArray[0] = messageArray[5];
                colorArray[1] = messageArray[6];
                colorArray[2] = messageArray[7];
                nB = BitToByte(colorArray);

                nColor = Color.FromArgb(nR, nG, nB);
                src.SetPixel(0, i + 1, nColor);
            }
        }


        private int GetCountText(Bitmap src)
        {
            byte res = 0;
            Color color = src.GetPixel(1, 0);
            BitArray colorArray = ByteToBit(color.R); //получаем байт цвета и преобразуем в массив бит
            BitArray messageArray = ByteToBit(color.R); ;//инициализируем результирующий массив бит
            messageArray[0] = colorArray[0];
            messageArray[1] = colorArray[1];

            colorArray = ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
            messageArray[2] = colorArray[0];
            messageArray[3] = colorArray[1];
            messageArray[4] = colorArray[2];

            colorArray = ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
            messageArray[5] = colorArray[0];
            messageArray[6] = colorArray[1];
            messageArray[7] = colorArray[2];
            res = BitToByte(messageArray);

            string result = string.Empty;

            for (int i = 0; i < res; i++)
            {
                color = src.GetPixel(0, i + 1);
                colorArray = ByteToBit(color.R);//получаем байт цвета и преобразуем в массив бит
                messageArray[0] = colorArray[0];
                messageArray[1] = colorArray[1];

                colorArray = ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
                messageArray[2] = colorArray[0];
                messageArray[3] = colorArray[1];
                messageArray[4] = colorArray[2];

                colorArray = ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
                messageArray[5] = colorArray[0];
                messageArray[6] = colorArray[1];
                messageArray[7] = colorArray[2];

                result += BitToByte(messageArray).ToString();
            }

            return Convert.ToInt32(result);
        }

        #region Внедрение_рейт
        private void WriteReit(int Reit, Bitmap src)
        {
            byte[] Symbol = Encoding.ASCII.GetBytes(Reit.ToString());
            BitArray ArrBeginSymbol = ByteToBit(Symbol[0]);
            Color curColor = src.GetPixel(2, 0);
            BitArray tempArray = ByteToBit(curColor.R);
            tempArray[0] = ArrBeginSymbol[0];
            tempArray[1] = ArrBeginSymbol[1];
            byte nR = BitToByte(tempArray);

            tempArray = ByteToBit(curColor.G);
            tempArray[0] = ArrBeginSymbol[2];
            tempArray[1] = ArrBeginSymbol[3];
            tempArray[2] = ArrBeginSymbol[4];
            byte nG = BitToByte(tempArray);

            tempArray = ByteToBit(curColor.B);
            tempArray[0] = ArrBeginSymbol[5];
            tempArray[1] = ArrBeginSymbol[6];
            tempArray[2] = ArrBeginSymbol[7];
            byte nB = BitToByte(tempArray);

            Color nColor = Color.FromArgb(nR, nG, nB);
            src.SetPixel(2, 0, nColor);
        }

        private int GetReit(Bitmap src)
        {
            byte[] rez = new byte[1];
            Color color = src.GetPixel(2, 0);
            BitArray colorArray = ByteToBit(color.R); //получаем байт цвета и преобразуем в массив бит
            BitArray messageArray = ByteToBit(color.R); ;//инициализируем результирующий массив бит
            messageArray[0] = colorArray[0];
            messageArray[1] = colorArray[1];

            colorArray = ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
            messageArray[2] = colorArray[0];
            messageArray[3] = colorArray[1];
            messageArray[4] = colorArray[2];

            colorArray = ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
            messageArray[5] = colorArray[0];
            messageArray[6] = colorArray[1];
            messageArray[7] = colorArray[2];
            rez[0] = BitToByte(messageArray); //получаем байт символа, записанного в 1 пикселе
            string m = Encoding.ASCII.GetString(rez);

            return Convert.ToInt32(m);
        }
        #endregion
        #endregion

        /* LSB - R */
        public Bitmap EncodeSteg(Bitmap bPic, Stream rText, int Reit)
        {
            BinaryReader bText = new BinaryReader(rText, Encoding.ASCII);
            List<byte> bList = new List<byte>();
            while (bText.PeekChar() != -1)
            { //считали весь текстовый файл для шифрования в лист байт
                bList.Add(bText.ReadByte());
            }
            bText.Close();

            //проверяем, поместиться ли исходный текст в картинке
            if (bList.Count > ((bPic.Width * bPic.Height * 3)) - 8 - 8)
            {
                MessageBox.Show("Выбранная картинка мала для размещения выбранного текста", "Информация", MessageBoxButtons.OK);
                return null;
            }

            //проверяем, может быть картинка уже зашифрована
            if (isEncryption(bPic))
            {
                MessageBox.Show("Файл уже зашифрован", "Информация", MessageBoxButtons.OK);
                return null;
            }

            byte[] Symbol = Encoding.GetEncoding(1251).GetBytes(startSymbol);
            BitArray ArrBeginSymbol = ByteToBit(Symbol[0]);
            Color curColor = bPic.GetPixel(0, 0);
            BitArray tempArray = ByteToBit(curColor.R);
            tempArray[0] = ArrBeginSymbol[0];
            tempArray[1] = ArrBeginSymbol[1];
            byte nR = BitToByte(tempArray);

            tempArray = ByteToBit(curColor.G);
            tempArray[0] = ArrBeginSymbol[2];
            tempArray[1] = ArrBeginSymbol[3];
            tempArray[2] = ArrBeginSymbol[4];
            byte nG = BitToByte(tempArray);

            tempArray = ByteToBit(curColor.B);
            tempArray[0] = ArrBeginSymbol[5];
            tempArray[1] = ArrBeginSymbol[6];
            tempArray[2] = ArrBeginSymbol[7];
            byte nB = BitToByte(tempArray);

            Color nColor = Color.FromArgb(nR, nG, nB);
            bPic.SetPixel(0, 0, nColor);

            WriteCountText(bList.Count, bPic); //записываем количество символов исходного текста
            WriteReit(Reit, bPic);

            bool[] allMessage = new bool[bList.Count * 8];
            int s = 0;

            for (int i = 0; i < bList.Count; i++)
            {
                BitArray temp = ByteToBit(bList[i]);
                for (int k = 0; k < 8; k++) allMessage[s++] = temp[k];
            }
            s = 0;

            for (int i = 3; i < bPic.Width; i++)
            {
                for (int j = 0; j < bPic.Height; j += Reit)
                {
                    Color pixelColor = bPic.GetPixel(i, j);

                    BitArray colorArray = ByteToBit(pixelColor.R);
                    if (s != (allMessage.Length - 1)) colorArray[0] = allMessage[s++];
                    byte newR = BitToByte(colorArray);

                    colorArray = ByteToBit(pixelColor.G);
                    if (s != (allMessage.Length - 1)) colorArray[0] = allMessage[s++];
                    byte newG = BitToByte(colorArray);

                    colorArray = ByteToBit(pixelColor.B);
                    if (s != (allMessage.Length - 1)) colorArray[0] = allMessage[s++];
                    byte newB = BitToByte(colorArray);

                    Color newColor = Color.FromArgb(newR, newG, newB);

                    bPic.SetPixel(i, j, newColor);
                    if (s == (allMessage.Length - 1)) goto Finish;
                }
            }
            Finish: return bPic;
        }

        /* LSB - M */
        public Bitmap EncodeStegM(Bitmap bPic, Stream rText, int Reit)
        {
            BinaryReader bText = new BinaryReader(rText, Encoding.ASCII);
            List<byte> bList = new List<byte>();
            while (bText.PeekChar() != -1)
            { //считали весь текстовый файл для шифрования в лист байт
                bList.Add(bText.ReadByte());
            }
            bText.Close();

            //проверяем, поместиться ли исходный текст в картинке
            if (bList.Count > ((bPic.Width * bPic.Height * 3)) - 8 - 8)
            {
                MessageBox.Show("Выбранная картинка мала для размещения выбранного текста", "Информация", MessageBoxButtons.OK);
                return null;
            }

            //проверяем, может быть картинка уже зашифрована
            if (isEncryption(bPic))
            {
                MessageBox.Show("Файл уже зашифрован", "Информация", MessageBoxButtons.OK);
                return null;
            }

            byte[] Symbol = Encoding.GetEncoding(1251).GetBytes(startSymbol);
            BitArray ArrBeginSymbol = ByteToBit(Symbol[0]);
            Color curColor = bPic.GetPixel(0, 0);
            BitArray tempArray = ByteToBit(curColor.R);
            tempArray[0] = ArrBeginSymbol[0];
            tempArray[1] = ArrBeginSymbol[1];
            byte nR = BitToByte(tempArray);

            tempArray = ByteToBit(curColor.G);
            tempArray[0] = ArrBeginSymbol[2];
            tempArray[1] = ArrBeginSymbol[3];
            tempArray[2] = ArrBeginSymbol[4];
            byte nG = BitToByte(tempArray);

            tempArray = ByteToBit(curColor.B);
            tempArray[0] = ArrBeginSymbol[5];
            tempArray[1] = ArrBeginSymbol[6];
            tempArray[2] = ArrBeginSymbol[7];
            byte nB = BitToByte(tempArray);

            Color nColor = Color.FromArgb(nR, nG, nB);
            bPic.SetPixel(0, 0, nColor);

            WriteCountText(bList.Count, bPic); //записываем количество символов исходного текста
            WriteReit(Reit, bPic);

            bool[] allMessage = new bool[bList.Count * 8];
            int s = 0;

            for (int i = 0; i < bList.Count; i++)
            {
                BitArray temp = ByteToBit(bList[i]);
                for (int k = 0; k < 8; k++) allMessage[s++] = temp[k];
            }
            s = 0;
            Random rand = new Random();


            for (int i = 3; i < bPic.Width; i++)
            {
                for (int j = 0; j < bPic.Height; j += Reit)
                {
                    Color pixelColor = bPic.GetPixel(i, j);

                    BitArray colorArray = ByteToBit(pixelColor.R);
                    byte newR = pixelColor.R;
                    if (s != (allMessage.Length - 1))
                    {
                        if (pixelColor.R == 0 || pixelColor.R == Byte.MaxValue)
                        {
                            colorArray[0] = allMessage[s++];
                            newR = BitToByte(colorArray);
                        }
                        else
                        {
                            if (colorArray[0] != allMessage[s])
                            {
                                int res = rand.Next(2);
                                byte nc = pixelColor.R;
                                if (res == 0) --nc; else ++nc;
                                newR = nc;
                            }
                            s++;
                        }
                    }
                    //File.AppendAllText("TESTM.txt", newR.ToString() + "!" + pixelColor.R + " ");

                    colorArray = ByteToBit(pixelColor.G);
                    byte newG = pixelColor.G;
                    if (s != (allMessage.Length - 1))
                    {
                        if (pixelColor.G == 0 || pixelColor.G == Byte.MaxValue)
                        {
                            colorArray[0] = allMessage[s++];
                            newG = BitToByte(colorArray);
                        }
                        else
                        {
                            if (colorArray[0] != allMessage[s])
                            {
                                int res = rand.Next(2);

                                byte nc = pixelColor.G;
                                if (res == 0) --nc; else ++nc;
                                newG = nc;
                            }
                            s++;
                        }
                    }
                    //File.AppendAllText("TESTM.txt", newG.ToString() + "!" + pixelColor.G + " ");

                    colorArray = ByteToBit(pixelColor.B);
                    byte newB = pixelColor.B;
                    if (s != (allMessage.Length - 1))
                    {
                        if (pixelColor.B == 0 || pixelColor.B == Byte.MaxValue)
                        {
                            colorArray[0] = allMessage[s++];
                            newB = BitToByte(colorArray);

                            //File.AppendAllText("TESTM.txt", newB.ToString() + "&" + pixelColor.B + " ");
                        }
                        else
                        {
                            if (colorArray[0] != allMessage[s])
                            {
                                int res = rand.Next(2);
                                byte nc = pixelColor.B;
                                if (res == 0) --nc; else ++nc;

                                newB = nc;

                                //File.AppendAllText("TESTM.txt", newB.ToString() + "!" + pixelColor.B + " ");
                            }
                            s++;
                        }
                    }

                    Color newColor = Color.FromArgb(newR, newG, newB);
                    bPic.SetPixel(i, j, newColor);
                    if (s == (allMessage.Length - 1)) goto Finish;
                }
            }
            Finish: return bPic;
        }

        /* Дешифрование */
        public string DecodeSteg(Bitmap rFile)
        {
            #region Чтение_данных
            Bitmap bPic = new Bitmap(rFile);
            if (!isEncryption(bPic))
            {
                MessageBox.Show("В файле нет зашифрованной информации", "Информация", MessageBoxButtons.OK);
                return null;
            }

            int countSymbol = GetCountText(bPic); //считали количество  символов
            int Reit = GetReit(bPic);

            bool[] allMessage = new bool[countSymbol * 8];
            int s = 0;
            #endregion

            #region Извлечения_сообщения_из_картинки
            for (int i = 3; i < bPic.Width; i++)
            {
                for (int j = 0; j < bPic.Height; j += Reit)
                {
                    Color pixelColor = bPic.GetPixel(i, j);

                    BitArray colorArray = ByteToBit(pixelColor.R);
                    if (s != (allMessage.Length - 1)) allMessage[s++] = colorArray[0];

                    colorArray = ByteToBit(pixelColor.G);
                    if (s != (allMessage.Length - 1)) allMessage[s++] = colorArray[0];

                    colorArray = ByteToBit(pixelColor.B);
                    if (s != (allMessage.Length - 1)) allMessage[s++] = colorArray[0];

                    if (s == allMessage.Length - 1) goto Finish;
                }
            }
            #endregion

            #region Преобразование_сообщения_в_корректный_вид
            Finish:
            byte[] message = new byte[countSymbol * 8];
            s = 0;

            for (int i = 0; i < message.Length; i++)
            {
                BitArray temp = new BitArray(8);
                for (int k = 0; k < 8; k++) if (s != (message.Length)) temp[k] = allMessage[s++];
                message[i] = BitToByte(temp);
            }
            string strMessage = Encoding.GetEncoding(1251).GetString(message);
            #endregion
            return strMessage;
        }
    }
}
