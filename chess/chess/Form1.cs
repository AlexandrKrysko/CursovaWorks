using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Resources;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Threading;

namespace chess
{
    public partial class Form1 : Form
    {
    public Form1()
        {
            InitializeComponent();//Розставляю айтеми по формі
        }
    private struct cells //Описую структуру шахової дошки
        {   
           public  string name; //Ім'я фігури
           public  string BorW;// Колір фігури
           public int positionX;//координати по Х
           public int positionY;// Координати по У
           public string pos_BorW;//Колір клітинки на якій стоїть фігура
           public bool firstMove;// перевірк першого кроку - необхідно для пєшки
           
        };
    private static cells[,] pole; //виділяю пам'ять для структури
    private static Point position; //Тут будуть знаходитись координати фігури вибраної для ходу
    private static bool stat=false; //Змінна стану
    private static int daeth_white = 0, daeth_black = 0;//лічильник збитих фігур
    private static string player="Білий";// Якщо тру, то ходить білий
    private void fill_figurs(bool flag) //Розставляю фігури на полі
    { 
        if (flag) //Якщо flag = true Розставляю чорні фігури
        {
            pole[0, 0].name = "Тура"; //Кожній фігури присвоюю ім'я
            pole[1, 0].name = "Кінь";
            pole[2, 0].name = "Слон";
            pole[3, 0].name = "Королева";
            pole[4, 0].name = "Король";
            pole[5, 0].name = "Слон";
            pole[6, 0].name = "Кінь";
            pole[7, 0].name = "тура";
            for(int i = 0; i < 8; i++)
            {
                pole[i, 1].name = "Пєшка";
                pole[i, 1].firstMove = true;
                pole[i, 1].BorW = "Чорний"; //..і колір
                pole[i, 0].BorW = "Чорний";
                
            }
        }
        else//білі
        {
            pole[0, 7].name = "Тура";
            pole[1, 7].name = "Кінь";
            pole[2, 7].name = "Слон";
            pole[3, 7].name = "Королева";
            pole[4, 7].name = "Король";
            pole[5, 7].name = "Слон";   //те ж саме, тільки для білих
            pole[6, 7].name = "Кінь";
            pole[7, 7].name = "Тура";
            for (int i = 0; i < 8; i++)
            {
                pole[i, 6].name = "Пєшка";
                pole[i, 6].firstMove = true;
                pole[i, 6].BorW = "Білий";
                pole[i, 7].BorW = "Білий";
            }
        }
    }
    private void start()//початкова розстановка
        {
            pole = new cells[8,8]; //pole[x,y] Виділяю динамічну пам'ять для шахової дошки
            //Насправді - це просто матриця із структури cells розміром 8 на 8
            fill_figurs(true); //Викликаю ф-цію розстановки чорних фігур
            fill_figurs(false);// для білих фігур
            int count = 1; //ця змінна визначає якого кольору клітка
            int coordinataX=0,coordinataY=0; //Задаю початкове значення
            for (int y = 0; y < 8; y++) //Заповнення поля чорними і білими клітинками
            {
                for(int x = 0; x < 8; x++)
                    {   if (count % 2 > 0)//Якщо непарне
                        pole[x, y].pos_BorW = "White"; //То біле
                    else
                        pole[x, y].pos_BorW = "Black"; // Якщо парне, то чорне
                    count++; //плюсую лічильник
                    pole[x, y].positionX = coordinataX;//Встановлюю координати місцезнаходження фігури
                    pole[x, y].positionY = coordinataY;//на полі
                    coordinataX += 30; //Малюнки, що загружаються - це квадрати 30х30 тому +30
                    }
                count++;
                coordinataY += 30;
                coordinataX = 0; //обнуляю лічильник для х
            }
        }
     private void drawing() //Ф-ція котра буде почергово перебирати елементи матриці
                            // і в залежності від параметрів і координат елемента 
                            //загружати в пам'ять картнку з відповідним зображенням і
                            //малювати його на заданій графічній області
        {
            Graphics board = this.panel1.CreateGraphics(); //Оголошую графічну область
            Bitmap white_check = new Bitmap(@"figurs\БілийКлетка.bmp"); //завантажую картинку пустої клітинки
            Bitmap black_check = new Bitmap(@"figurs\ЧорнийКлетка.bmp");
            Bitmap temp;//змінна ддля тимчасового зберігання завантаених картинок
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    if (pole[x, y].name != null) //Якщо поле name елемента не пусте, то це якась фігура
                    {//завантажую клітинку з відповідною фігурою
                        temp = new Bitmap(@"figurs\" + pole[x, y].BorW + pole[x, y].name + "На" + pole[x, y].pos_BorW + ".bmp");
                        board.DrawImage(temp, pole[x, y].positionX, pole[x, y].positionY); //малюю
                        temp.Dispose();//звільнюю тимчасову змінну
                    }
                    else //якщо поле name елемента пусте, то це порожня клітинка
                    {
                        if (pole[x, y].pos_BorW == "White") //Визначаю колір клітинки
                            board.DrawImage(white_check, pole[x, y].positionX, pole[x, y].positionY); //малюю
                        else
                            board.DrawImage(black_check, pole[x, y].positionX, pole[x, y].positionY); 
                    }
            white_check.Dispose(); //звільнюю пам'ять яку займав
            black_check.Dispose();
            board.Dispose();
     }
     private void go(int x, int y) //Ф-кція переміщення фігур по полю
     {
         if (pole[position.X, position.Y].BorW == player) //перевіряю, який гравець зараз ходить
         {                                                 //І фігуру якого кольору він хоче рухати
             if (pole[x, y].name != null) //Якщо гравець хоче когось збити
             {
                 if (pole[x, y].BorW != pole[position.X, position.Y].BorW)//Перевіряю, чи хоче він збити фігуру
                 {
                     MessageBox.Show("Ви збили фігуру підз назвою: " + pole[x, y].name+":)");//Якщо все норм, то виводжу повідомлення
                     if (pole[x, y].BorW == "Білий") //Збив, підвищую лічильник збитих фігур на 1
                         daeth_white++;
                     else
                         daeth_black++;
                         
                     pole[x, y].name = pole[position.X, position.Y].name; //Переміщую фігуру
                     pole[x, y].BorW = pole[position.X, position.Y].BorW;
                     pole[position.X, position.Y].name = null; //зануляю непотрібні поля
                     pole[position.X, position.Y].BorW = null;
                 }
             }
             else //якщо гравець просто ходить
             {
                 pole[x, y].name = pole[position.X, position.Y].name; //переставляю фігуру на пусте місце
                 pole[x, y].BorW = pole[position.X, position.Y].BorW;
                 pole[position.X, position.Y].name = null;
                 pole[position.X, position.Y].BorW = null;
             }
             if (player == "Білий") //Встановлюється почерговість ходів
                 player = "Чорний";
             else
                 player = "Білий";
             label1.Text = "Збито білих "+daeth_white.ToString();//Виваджу всяку інфу
             label2.Text = "Збито чорних "+daeth_black.ToString();
             label3.Text = "Зараз ходить " + player + " гравець";
             drawing();
         }
         else MessageBox.Show("Зараз ходить: "+ player+" гравець");//якщо гравець намагається передвинути чужу фігуру, то виводжу нагадування
         proc_check();
     }
        private void button1_Click_1(object sender, EventArgs e)//обработчик события при нажатии на кнопку "Начать сначала!"
        {
            start();//Заповнюю захову дошку
            drawing();//виводжу поле
            daeth_white = 0;//обнуляю лічильник
            daeth_black = 0;
            player="Білий";//встановлюю гравця, котрий буде ходити першим
            label1.Text = "Збито білих " + daeth_white.ToString(); //виводжу інфу
            label2.Text = "Збито чорних " + daeth_black.ToString();
            label3.Text = "Зараз ходить " + player + " гравець";
        }
        private void maxmin(ref int max,ref int min,int x,int y,bool x_or_y)//ф-ція для знаходження максимальної і мінімальної координат
            //параметри мін і макс передаються за ссилкою, отже вони змінюються в пам'яті викликаючу функцію, а не в локальній
        { if(x_or_y) //якщо тру, то порівнюю координати по Х
            {
            if (position.X > x)
                    {
                        min = x; //знаходжу мінімум і присвоюю
                        max = position.X;
                    }
                    else
                    {
                        min = position.X+1;
                        max = x;
                    }
            }
        else //якщо false для У
        {
            if (position.Y > y) //те ж саме, що і вище тільки для У
                    {
                        min = y;
                        max = position.Y;
                    }
                    else
                    {
                        min = position.Y+1;
                        max = y;
                    }

        }
        }
        private int check_moove(int x, int y, out bool marker) //X Y координати куди ходити
        {//функція перевірки корректності всіх ходів
            bool flag = true;
            bool truk = false;
            marker = false;
            int min=0, max=0;
            int i;
            //В основі перевірки лежать логічні алгоритми(для кожної фігури свій алгоритм)
             if(pole[position.X, position.Y].name == "Пєшка")//Перевіряю, якщо вибрана пєшка, то перевіряю далі
              //якщо ні - перевіряю яка фігура
            {   //перевіряю правильність ходів для пєшки
                    if ( (position.X == x &&  (Math.Abs(position.Y - y) == 2 || Math.Abs(position.Y - y) == 1) ) && pole[x, y].name == null && pole[position.X,position.Y].firstMove)//тоді хід корректний
                    {  //Пєшка ходить на 2 або 1 клітинки вперше і збиває не далі однієї клітинки
                        //Тому перевіряєм координати по х і перевіряю різницю між ними
                        //і поточну координату У, якщ вона не = 1, то хід не є корректним
                        //якщо все ок, то 
                        pole[position.X, position.Y].firstMove = false;
                        go(x,y); //преміщую фігуру, Якщо всі умови виконані
                        return 1;//виходжу із функції
                    }
                    else if ((position.X == x && Math.Abs(position.Y - y) == 1) && pole[x, y].name == null && !(pole[position.X, position.Y].firstMove))//тоді хід корректний
                    {  //Пєшка ходить на 1-ну клітинку і збиває не далі однієї клітинки
                        //Тому перевіряєм координати по х і перевіряю різницю між ними
                        //і поточну координату У, якщ вона не = 1, то хід не є корректним
                        //якщо все ок, то
                        go(x, y); //преміщую фігуру, Якщо всі умови виконані
                        return 1;//виходжу із функції
                    }
                    else if ((Math.Abs(position.X - x) == 1) && Math.Abs(position.Y - y) == 1 && pole[x, y].name == "Король" && (pole[x, y].BorW != pole[position.X, position.Y].BorW))
                    {//тут перевіряється можливість збити фігуру, Враховується все те що і вище + перевіряється 
                        //поле name клітинки куди планується поставити пєшку, якщо поле name не дорівнює null(пустоті)
                        //то хід коректний
                        marker = true;
                        return 1;//Виходжу із функції
                    }
                    else if ((Math.Abs(position.X - x) == 1) && Math.Abs(position.Y - y) == 1 && pole[x, y].name != null)
                    {//тут перевіряється можливість збити фігуру, Враховується все те що і вище + перевіряється 
                        //поле name клітинки куди планується поставити пєшку, якщо поле name не дорівнює null(пустоті)
                        //то хід коректний
                        go(x, y); //переміщую фігуру, якщо виконані всі умови
                        return 1;//Виходжу із функції
                    }
            }
        if (pole[position.X, position.Y].name == "Тура")
            {
                if (position.X != x && position.Y == y) //хід по х
                {
                    maxmin(ref max,ref min,x,y,true);
                    if (pole[x, y].name == "Король" && (pole[x, y].BorW != pole[position.X, position.Y].BorW))
                    {
                        marker = true;
                        flag = false;
                    }
                    if (pole[x, y].name != null)
                    {
                        truk = true;
                        min++;
                    }
                    for (i = min; i < max; i++)
                        if (pole[i, y].name != null)
                        {
                            flag = false;
                            break;
                        }
                    
                    if ((flag && truk) || flag)
                    {go(x, y);
                        return 1;
                    }
                }
                else 
                    if (position.X == x && position.Y != y)
                        {
                            maxmin(ref max,ref min,x,y,false);
                            if (pole[x, y].name == "Король" && (pole[x, y].BorW != pole[position.X, position.Y].BorW))
                            {
                                marker = true;
                                flag = false;
                            }
                            if (pole[x, y].name != null)
                            {
                                truk = true;
                                min++;
                            }
                            for (i = min; i < max; i++) //хід по У
                                if (pole[x, i].name != null )
                                {
                                    flag = false;
                                    break;
                                }
                           
                            if ((flag&&truk) ||flag)
                            { go(x, y);
                              return 1;
                            }
                         }
                }      

    if (pole[position.X, position.Y].name == "Кінь")
        {
            if ((Math.Abs(position.X - x) == 2 && Math.Abs(position.Y - y) == 1) || (Math.Abs(position.X - x) == 1 && Math.Abs(position.Y - y) == 2) && pole[x, y].name == "Король" && (pole[x, y].BorW != pole[position.X, position.Y].BorW))
            {
                marker = true;
            }
            if ((Math.Abs(position.X - x) == 2 && Math.Abs(position.Y - y) == 1) || (Math.Abs(position.X - x) == 1 && Math.Abs(position.Y - y) == 2) && pole[x,y].name != null)
            {go(x, y);
            return 1;
            }
        }
    if (pole[position.X, position.Y].name == "Слон")
        {
            int min_y=0, min_x=0, max_y=0, max_x=0;
            if (position.X != x && position.Y != y && pole[position.X,position.Y].pos_BorW==pole[x,y].pos_BorW)
            {
                maxmin(ref max_x,ref min_x,x,y,true);
                maxmin(ref max_y,ref min_y,x,y,false);
                if (pole[x, y].name == "Король" && (pole[x, y].BorW != pole[position.X, position.Y].BorW))
                {
                    marker = true;
                    flag = false;
                }
                if (pole[x, y].name != null)
                {   truk = true;
                    min_x++;
                    min_y++;
                }
                for (i = min_x; i < max_x; i++, min_y++)
                    if (pole[i, min_y].name != null)
                    {
                        flag = false;
                        break;
                    }
                if ((flag && truk) ||flag) 
                {   go(x, y);
                drawing();
                    return 1;
                }
            }
        }
        if (pole[position.X, position.Y].name == "Король")
        {
            if (Math.Abs(position.X - x) <= 1 && Math.Abs(position.Y - y) <= 1)
            {
                go(x,y);
                drawing();
                return 1;
            }
        }
        if (pole[position.X, position.Y].name == "Королева")
        {
            if (position.X != x && position.Y == y) //хід по х
            {
                maxmin(ref max, ref min, x, y, true);
                if (pole[x, y].name == "Король" && (pole[x, y].BorW != pole[position.X, position.Y].BorW))
                {
                    marker = true;
                    flag = false;
                }
                if (pole[x, y].name != null)
                {
                    truk = true;
                    min++;
                }
                for (i = min; i < max; i++)
                    if (pole[i, y].name != null)
                    {
                        flag = false;
                        break;
                    }

                if ((flag && truk) || flag)
                {
                    go(x, y);
                    return 1;
                }
            }
            else
                if (position.X == x && position.Y != y)
                {
                    maxmin(ref max, ref min, x, y, false);
                    if (pole[x, y].name == "Король" && (pole[x, y].BorW != pole[position.X, position.Y].BorW))
                    {
                        marker = true;
                        flag = false;
                    }
                    if (pole[x, y].name != null)
                    {
                        truk = true;
                        min++;
                    }
                    for (i = min; i < max; i++) //хід по У
                        if (pole[x, i].name != null)
                        {
                            flag = false;
                            break;
                        }

                    if ((flag && truk) || flag)
                    {
                        go(x, y);
                        return 1;
                    }
                }
            int min_y = 0, min_x = 0, max_y = 0, max_x = 0;
            if (position.X != x && position.Y != y && pole[position.X, position.Y].pos_BorW == pole[x, y].pos_BorW)
            {
                maxmin(ref max_x, ref min_x, x, y, true);
                maxmin(ref max_y, ref min_y, x, y, false);
                if (pole[x, y].name == "Король" && (pole[x, y].BorW != pole[position.X, position.Y].BorW))
                {
                    marker = true;
                    flag = false;
                }
                if (pole[x, y].name != null)
                {
                    truk = true;
                    min_x++;
                    min_y++;
                }
                for (i = min_x; i < max_x; i++, min_y++)
                    if (pole[i, min_y].name != null)
                    {
                        flag = false;
                        break;
                    }
                if ((flag && truk) || flag)
                {
                    go(x, y);
                    drawing();
                    return 1;
                }
            }
        }
        
         stat = false;
            drawing();
            return 0;        
    }

        private void check_check(int x, int y, out bool marker)
        {
            marker = false;
            if (pole[x, y].name == "Пєшка")
            {
                if (pole[Math.Abs(x - 1), Math.Abs(y - 1)].name == "Король" && pole[Math.Abs(x - 1), Math.Abs(y - 1)].BorW != pole[x, y].BorW)
                {
                    marker = true;
                }
            }
            if (pole[x, y].name == "Тура")
            {
                for ( int i = 0; i <8; i++) 
                {
                    if (pole[i, y].name != "Король" && pole[i, y].name != null && pole[i, y].BorW != pole[x, y].BorW && (x - i <= 0)) continue;
                    if (pole[i, y].name != "Король" && pole[i, y].name != null && pole[i, y].BorW != pole[x, y].BorW && (x - i > 0)) break;
                    else
                        if (pole[i, y].name == "Король" && pole[i, y].BorW != pole[x, y].BorW)
                        {
                            marker = true;
                        }
                }
                for (int i = 0; i < 8; i++)
                    {
                        if (pole[x, i].name != "Король" && pole[x, i].name != null && pole[x, i].BorW != pole[x, y].BorW && (y - i < 0)) continue;
                        if (pole[x, i].name != "Король" && pole[x, i].name != null && pole[x, i].BorW != pole[x, y].BorW && (y - i > 0)) break;
                        else
                            if (pole[x, i].name == "Король" && pole[x, i].BorW != pole[x, y].BorW)
                            {
                                marker = true;
                            }
                    }    
            }

            if (pole[x, y].name == "Кінь")
            {
                if ( (pole[Math.Abs(x - 1), Math.Abs(y - 2)].name == "Король" && pole[Math.Abs(x - 1), Math.Abs(y - 2)].BorW != pole[x, y].BorW) || (pole[Math.Abs(x - 2), Math.Abs(y - 1)].name == "Король" && pole[Math.Abs(x - 2), Math.Abs(y - 1)].BorW != pole[x, y].BorW) )
                {
                    marker = true;
                }
            }
            if (pole[x, y].name == "Слон")
            {
                for (int i = 0; i < 8; i++)
                {
                    if (pole[(x + i) % 7, (y + i) % 7].name != "Король" && ( pole[(x + i) % 7, (y + i) % 7].name != null && pole[(x + i) % 7, (y + i) % 7].BorW != pole[x, y].BorW))
                        continue;
                    else if (pole[(x + i) % 7, (y + i) % 7].name == "Король" && pole[(x + i) % 7, (y + i) % 7].BorW != pole[x, y].BorW)
                    {
                        marker = true;
                    }
                }
            }
            if (pole[x, y].name == "Королева")
            {
                for (int i = 0; i < 8; i++)
                {
                    if (pole[i, y].name != "Король" && pole[i, y].name != null && pole[i, y].BorW != pole[x, y].BorW && (x - i <= 0)) continue;
                    if (pole[i, y].name != "Король" && pole[i, y].name != null && pole[i, y].BorW != pole[x, y].BorW && (x - i > 0)) break;
                    else
                        if (pole[i, y].name == "Король" && pole[i, y].BorW != pole[x, y].BorW)
                        {
                            marker = true;
                        }
                }
                for (int i = 0; i < 8; i++)
                {
                    if (pole[x, i].name != "Король" && pole[x, i].name != null && pole[x, i].BorW != pole[x, y].BorW && (y - i < 0)) continue;
                    if (pole[x, i].name != "Король" && pole[x, i].name != null && pole[x, i].BorW != pole[x, y].BorW && (y - i > 0)) break;
                    else
                        if (pole[x, i].name == "Король" && pole[x, i].BorW != pole[x, y].BorW)
                        {
                            marker = true;
                        }
                }    
                for (int i = 0; i < 8; i++)
                {
                    if (pole[(x + i) % 7, (y + i) % 7].name != "Король" && (pole[(x + i) % 7, (y + i) % 7].name != null && pole[(x + i) % 7, (y + i) % 7].BorW != pole[x, y].BorW ))
                        continue;
                    else    if (pole[(x + i) % 7, (y + i) % 7].name == "Король" && pole[(x + i) % 7, (y + i) % 7].BorW != pole[x, y].BorW)
                    {
                        marker = true;
                    }
                }
            }
        }

      private int check_Koordinate(int c) //в цю ф-цію передаються координати клітинок в пікселях
        {               //потрібно перевести їх в індекси масиву
                if (c < 30) return 0; //якщо передане число менше 30 то індекс = 0
                if (c > 30 && c < 60) return 1;
                if (c > 60 && c < 90) return 2;
                if (c > 90 && c < 120) return 3;
                if (c > 120 && c < 150) return 4;
                if (c > 150 && c < 180) return 5;
                if (c > 180 && c < 210) return 6;
                if (c > 210 && c < 240) return 7;
                return -1;//Якщо координата виходить за межі, то повертаєм -1
            
        }
        private void panel1_MouseClick(object sender, MouseEventArgs e)//обробник події, котрий викликається при
            //натисканні на клітинку поля
      {
          if (check_Koordinate(e.X) >= 0 && check_Koordinate(e.Y) >= 0)//перевіряю чи не виходить координата за межі
            if (stat) //Якщо ні, то перевіряю, чи була вибрана фігура для ходу
            {
               //якщо була, то викликаю ф-цію перевірки корректності ходу
                    bool mark;
                    check_moove(check_Koordinate(e.X), check_Koordinate(e.Y), out mark);
                    if (mark)  MessageBox.Show("Check!");
                    stat = false;
            }
            else
            {
                    if (pole[check_Koordinate(e.X), check_Koordinate(e.Y)].name == null)
                        stat = false;//Дивлюсь, щоб гравець не рухав порожні клітинки
                    else
                    {
                        position.X = check_Koordinate(e.X);//Якщо гравець цього не збирався робити
                        position.Y = check_Koordinate(e.Y);// то все добре
                        stat = true;//Запам'ятовую координати вибраної для ходу фігури і чекаю, доки гравець вибере куди поставити фігуру
                    }
                }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            drawing();//обробник події зміни гооловної форми
                      //це робиться для того, щоб зображення дошки не зникало 
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            drawing();//малюю дошку при завантаженні
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            start();//Встановлюю початкові значення при увімкненні програми
            drawing();
            player = "Білий";
            label1.Text = "Збито білих " + daeth_white.ToString();
            label2.Text = "Збито чорних " + daeth_black.ToString();
            label3.Text = "Зараз ходить " + player + " Гравець";
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            drawing(); //малюю
        }

        private void proc_check()
        {
            bool marker = false;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if ( pole[i,j].name != null )
                    {
                        check_check(i, j, out marker);
                        if (marker)
                            MessageBox.Show("Check!");
                    }
                    
                }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            drawing(); //при перемалюванні дошка може зникнути, тому маллю ще раз
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form f4 = new Form4();
            Form f1 = new Form1();
            f4.Show();
            this.Close();

        }

        

       
    }
}