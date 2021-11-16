using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Tutorial_1
{
    /// Hola, soy Juan he hecho este programa para practicar mis habilidades de C# y .Net
    /// 

    public partial class MainWindow : Window
    {
        float f_money = 0;

        int i_Min_number_game = 0; //El rango del nº aleatorio de la lotería
        int i_MAx_number_game = 10;

        SqlConnection miConexionSql;


        public MainWindow()
        {
            InitializeComponent();

            im_BackGround.Stretch = Stretch.Fill;

            lb_BetNumber.Content = "(" + i_Min_number_game + "-" + i_MAx_number_game +")";
            tb_user.Text = "Default_user";
            //
            string miConexion = ConfigurationManager.ConnectionStrings["Tutorial_1.Properties.Settings.Casino_Database_ProyectoConnectionString"].ConnectionString;
            miConexionSql = new SqlConnection(miConexion);

            //
            SetUserComboBox();
            //
            lb_money.Content = f_money.ToString();

            NewGame();
        }

        private void NewGame()
        {
            //Reiniciar valores
            tb_number.Text = "0";
            tb_bet.Text = "0";
            bt_Result.Content = "?";
            lb_Resultado_Perder_Ganar.Content = "";
        }

        private void SetUserComboBox() //Carga los usuarios con Dinero en el ComboBox
        {
           
            string str_item_user = "";

            miConexionSql.Open();

            String str_NameUser;
            str_NameUser = String.Format("Select User_name From User_Data inner join User_score on User_score.idUser = User_Data.idUser where Money > 0", tb_user.Text);//
            SqlCommand command_ = new SqlCommand(str_NameUser, miConexionSql);
            SqlDataReader reader_ = command_.ExecuteReader();

            while (reader_.Read())
            {
                var x = reader_.GetValue(0);
                str_item_user = x.ToString();
                cb_user.Items.Add(str_item_user);
            }

            //
            miConexionSql.Close();

            cb_user.SelectedItem = " ";
        }

        private void Button_Click_Effect(object sender, RoutedEventArgs e)
        {
            //Conectarse a Internet, sino hay sale un vídeo por defecto

        }

        private void bt_Start_Click(object sender, RoutedEventArgs e)
        {
            //Conectarse a una base de Datos y actualizar la puntuación
            Check_User();
            
            bool b_validate = false;
            if (Convert.ToInt32(tb_number.Text) >= i_Min_number_game && Convert.ToInt32(tb_number.Text) <= i_MAx_number_game) //Nº Válido
            {
                b_validate = true;
            }
            else
            {
                MessageBox.Show("Nº Inválido. Solo nº de 0 al 10.");
            }

            //Quitar los 0 de la izquierda o dará error al comparar//Solo pensado para 0 al 10 //Ej: 00001,0002,02 -> 1,2

            char[] char_temp = tb_number.Text.ToCharArray();
            tb_number.Text = "";

            for (int i = 0; i < char_temp.Length-1; i++)
            {
                if (char_temp[i].Equals("0"))
                {
                    char_temp[i] = ' ';
                    tb_number.Text += char_temp[i];
                }
            }
            tb_number.Text += char_temp[char_temp.Length-1];

            //

            if (b_validate)
            {
                if (Convert.ToInt32(tb_bet.Text) > 0 && Convert.ToInt32(tb_bet.Text) < f_money)
                {
                    Random r = new Random();
                    int i_randomNumber = r.Next(i_Min_number_game, i_MAx_number_game + 1);
                    bt_Result.Content = i_randomNumber.ToString();

                    if (i_randomNumber != Convert.ToInt32(tb_number.Text))
                    {
                        lb_signo.Content = "-";
                        lb_Resultado_Perder_Ganar.Content = "HAS PERDIDO";

                        miConexionSql.Open();

                        string str_BetUser = String.Format("Update User_Score Set Money = (Money - {0}) from User_Score inner join User_Data On User_Data.idUser = User_Score.idUser where User_Data.User_name = '{1}'", Convert.ToInt32(tb_bet.Text), tb_user.Text);//('{0}')
                        SqlCommand command_ = new SqlCommand(str_BetUser, miConexionSql);
                        command_.ExecuteNonQuery();

                        miConexionSql.Close();
                    }
                    else
                    {
                        lb_signo.Content = "+";
                        lb_Resultado_Perder_Ganar.Content = "HAS GANADO";

                        /*var uriSource = new Uri("https://media.giphy.com/media/zqqGAer6AJCr6/giphy.gif");
                        im_fondo.Source = new BitmapImage(uriSource);*/

                        miConexionSql.Open();

                        string str_BetUser = String.Format("Update User_Score Set Money = (Money + ( {0} * 3) ) from User_Score inner join User_Data On User_Data.idUser = User_Score.idUser where User_Data.User_name = '{1}'", Convert.ToInt32(tb_bet.Text), tb_user.Text);//('{0}')
                        SqlCommand command_ = new SqlCommand(str_BetUser, miConexionSql);
                        command_.ExecuteNonQuery();

                        miConexionSql.Close();
                    }

                    //Sumar variable
                    int i_idUser = 0;

                    miConexionSql.Open();

                    string str_NameUser = String.Format("Select idUser From User_Data where User_name = ('{0}')", tb_user.Text);
                    SqlCommand command_3 = new SqlCommand(str_NameUser, miConexionSql);
                    SqlDataReader reader_3 = command_3.ExecuteReader();

                    using (reader_3)
                    {
                        reader_3.Read();

                        var strXXX = reader_3.GetValue(0);

                        i_idUser = Convert.ToInt32(strXXX);

                    }
                    miConexionSql.Close();

                    miConexionSql.Open();

                    string str_Cont = String.Format("Update User_Score Set VecesJugadasJuego1 = VecesJugadasJuego1 +1 where idUser = {0}", i_idUser);//
                    SqlCommand command_2 = new SqlCommand(str_Cont, miConexionSql);
                    command_2.ExecuteNonQuery();

                    miConexionSql.Close();

                    //Color
                    Random r2 = new Random();
                    int i_randomNumber2 = r2.Next(0, 150);
                    Random r3 = new Random();
                    int i_randomNumber3 = r2.Next(0, 150);
                    Random r4 = new Random();
                    int i_randomNumber4 = r2.Next(0, 150);

                    lb_Resultado_Perder_Ganar.Foreground = new SolidColorBrush(Color.FromRgb(Convert.ToByte(i_randomNumber2), Convert.ToByte(i_randomNumber3), Convert.ToByte(i_randomNumber4)));
                    lb_signo.Foreground = new SolidColorBrush(Color.FromRgb(Convert.ToByte(i_randomNumber2), Convert.ToByte(i_randomNumber3), Convert.ToByte(i_randomNumber4)));

                    //Actualizar el Label del dinero del usuario
                    miConexionSql.Open();

                    string str_consulta = String.Format("Select Money From User_Score inner join  User_Data on User_Score.idUser = User_Data.idUser where User_Data.User_name ='{0}'", tb_user.Text);// 'Default_user'
                                                                                                                                                                                                     //string str_consulta ="Select Money From User_Score inner join  User_Data on User_Score.idUser = User_Data.idUser where User_Data.idUser = @idUser_Selected ";
                    SqlCommand command = new SqlCommand(str_consulta, miConexionSql);

                    SqlDataReader reader = command.ExecuteReader();

                    using (reader)
                    {
                        reader.Read();

                        var strXXX = reader.GetValue(0);

                        f_money = Convert.ToInt32(strXXX);
                    }

                    miConexionSql.Close();

                    lb_money.Content = f_money.ToString();
                }
                else
                {
                    MessageBox.Show("La cantidad de la apuesta es inválida, tiene que ser mayor que 0 y menor o igual que la cantidad que posees. Crea nuevo usuario si no tiene suficiente puntos");//
                    tb_bet.Text = "1";
                }
            }
        }

        private void Check_User() //Crear usuario si no existe
        {
            //Sql ver si existe, sino lo crea
            //!!Hay que meter un usuario sin espacios y  con un mínimo de dos caracteres, y sin caracteres especiales. Comprobación:

            miConexionSql.Open();

            String str_NameUser;
            str_NameUser = String.Format("Select User_name From User_Data where User_name = ('{0}')", tb_user.Text);//
            SqlCommand command_ = new SqlCommand(str_NameUser, miConexionSql);
            SqlDataReader reader_ = command_.ExecuteReader();

            using (reader_)
            {
                reader_.Read();

                bool b_NewUser = false;

                try
                {
                    var x = reader_.GetValue(0);
                }
                catch (Exception ex) //No ha recibido ningún dato, esa consulta no ha devuelto nada //Trato de sacar un valor en var pero si la consulta no ha devuelto nada, dará error y sabré que no hay un usuario que se llame así.
                {
                    miConexionSql.Close();

                    //Añadir Usuario
                    miConexionSql.Open();

                    MessageBox.Show("Usuario Nuevo Detectado. Empezarás con 3000 unidades");

                    str_NameUser = String.Format("Insert into User_Data(User_name) values ('{0}')", tb_user.Text);
                    command_ = new SqlCommand(str_NameUser, miConexionSql);
                    command_.ExecuteNonQuery();

                    miConexionSql.Close();

                    //Iniciar User_Score
                    //1ºCogemos el idUser primero, en teoría si están ordenados esto no sería necesario, pero prefiero hacerlo así por si no esta ordenado o se borran usuarios              
                    int i_idUser = 0;

                    miConexionSql.Open();

                    str_NameUser = String.Format("Select idUser From User_Data where User_name = ('{0}')", tb_user.Text);
                    command_ = new SqlCommand(str_NameUser, miConexionSql);
                    reader_ = command_.ExecuteReader();

                    using (reader_)
                    {
                        reader_.Read();

                        var strXXX = reader_.GetValue(0);

                        i_idUser = Convert.ToInt32(strXXX);

                    }
                    miConexionSql.Close();

                    //Iniciar tabla User_Score
                    miConexionSql.Open();

                    str_NameUser = String.Format("Insert into User_Score(idUser) values ({0})", i_idUser);
                    command_ = new SqlCommand(str_NameUser, miConexionSql);
                    command_.ExecuteNonQuery();

                    miConexionSql.Close();

                    //Iniciar tabla User_Game_Data
                    miConexionSql.Open();

                    str_NameUser = String.Format("Insert into User_Game_Data(idUser) values ({0})", i_idUser);
                    command_ = new SqlCommand(str_NameUser, miConexionSql);
                    command_.ExecuteNonQuery();

                    miConexionSql.Close();

                    //
                    b_NewUser = true;
                }

                if (!b_NewUser)
                {
                    Console.WriteLine("Usuario Encontrado");

                    //!!Si ese personaje no tiene dinero, se le dará un mensaje "Usuario Inválido, no tiene Dinero, inserte un nuevo nombre"
                }
            }

            //MessageBox.Show(tb_user.Text);

            miConexionSql.Close();

            //Mostrar el dinero que le queda 
            miConexionSql.Open();

            string str_consulta = String.Format("Select Money From User_Score inner join  User_Data on User_Score.idUser = User_Data.idUser where User_Data.User_name ='{0}'", tb_user.Text);// 'Default_user'
            //string str_consulta ="Select Money From User_Score inner join  User_Data on User_Score.idUser = User_Data.idUser where User_Data.idUser = @idUser_Selected ";
            SqlCommand command = new SqlCommand(str_consulta, miConexionSql);

            /*
                        String str_selected = "1";

                        command.Parameters.AddWithValue("@idUser_Selected", str_selected);*/

            SqlDataReader reader = command.ExecuteReader();

            using (reader)
            {
                reader.Read();

                var strXXX = reader.GetValue(0);

                f_money = Convert.ToInt32(strXXX);
            }

            miConexionSql.Close();

            lb_money.Content = f_money.ToString();
        }

        private void Button_Click_HistoryUser(object sender, RoutedEventArgs e)
        {
            User_History m_user_History = new User_History();
            m_user_History.Show();
        }

        private void cb_user_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tb_user.Text = cb_user.SelectedItem.ToString();
        }
    }
}
