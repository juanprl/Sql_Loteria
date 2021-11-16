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
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Tutorial_1
{
    /// <summary>
    /// Lógica de interacción para User_History.xaml
    /// </summary>
    /// 
    public partial class User_History : Window
    {
        SqlConnection miConexionSql;

        public User_History()
        {
            InitializeComponent();

            im_background.Stretch = Stretch.Fill;

            string miConexion = ConfigurationManager.ConnectionStrings["Tutorial_1.Properties.Settings.Casino_Database_ProyectoConnectionString"].ConnectionString;
            miConexionSql = new SqlConnection(miConexion);

            //Cargar Datos
            String str_NameData;
            str_NameData = String.Format("Select User_Score.idUser, User_Score.Money, User_Score.VecesJugadasJuego1, User_Data.User_name  From User_Score, User_Data where User_Score.idUser = User_Data.idUser; ");
            SqlCommand command_ = new SqlCommand(str_NameData, miConexionSql);

            SqlDataAdapter adapter = new SqlDataAdapter(command_);
            DataTable dt_ = new DataTable();
            adapter.Fill(dt_);
            dg_userInfo.ItemsSource = dt_.DefaultView;

            //
            miConexionSql.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
