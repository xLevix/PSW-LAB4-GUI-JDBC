using MySqlConnector;

namespace Zadanie_1
{
    public partial class Form1 : Form
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string connectionString;
        int user_id;

        public Form1()
        {
            InitializeComponent();
            logowanie.Visible = false;
            rejestracja.Visible = false;
            login_approve_user.Visible = false;
            tabs.Visible = false;

            server = "mysql.mikr.us";
            database = "db_j166";
            uid = "j166";
            password = "DBF4_5b4db9";
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
        }

        private void register_Click(object sender, EventArgs e)
        {
            wybor.Visible = false;
            logowanie.Visible = false;
            rejestracja.Visible = true;
        }


        private void login_Click(object sender, EventArgs e)
        {
            wybor.Visible = false;
            logowanie.Visible = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                pole_haslo.PasswordChar = '\0';
            }
            else
            {
                pole_haslo.PasswordChar = '*';
            }
        }

        private void b_login_Click(object sender, EventArgs e)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = "SELECT * FROM users WHERE login = '" + pole_login.Text + "' AND haslo = '" + pole_haslo.Text + "'";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            if (dataReader.Read())
            {
                string uprawnienia = dataReader.GetString(6);
                if(uprawnienia == "user")
                {
                    login_approve_user.Visible = true;
                }
                else if (uprawnienia == "admin")
                {
                    tabs.Visible = true;
                }
                else
                {
                    MessageBox.Show("Niuprawniony do wszystkiego");
                }

                
                logowanie.Visible = false;
                wybor.Visible = true;
                user_id = dataReader.GetInt32("id");
            }
            else
            {
                MessageBox.Show("Niepoprawny login lub has³o");
            }

        }

        private void b_register_Click(object sender, EventArgs e)
        {
            if (rejestracja_haslo.Text == rejestracja_haslo2.Text)
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                string query = "INSERT INTO db_j166.users (imie, nazwisko, login, haslo, email, uprawnienia, data_rejestracji) " +
                    "VALUES ('" + rejestracja_imie.Text + "', '" + rejestracja_nazwisko.Text + "', '" + rejestracja_login.Text + "', '" + rejestracja_haslo.Text + "', '" + rejestracja_email.Text + "', '" + "user" + "', '" + DateTime.Now.ToString("yyyy-MM-dd") + "')";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Zarejestrowano");
                logowanie.Visible = false;
                wybor.Visible = true;

            }
            else
            {
                MessageBox.Show("Has³a nie s¹ takie same");
            }
            
           
        }


        Tuple<string, string, string>[] events;
        
        private void wydarzenie_DropDown(object sender, EventArgs e)
        {
            wydarzenie.Items.Clear();
            connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = "select count(*) from event";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            dataReader.Read();
            int ilosc = dataReader.GetInt32(0);
            dataReader.Close();
            events = new Tuple<string, string, string>[ilosc];
            int i = 1;
            for (int j = 0; i < ilosc+1; j++)
            {
                string zapytanie = "select nazwa, agenda, termin from event where id = " + i;
                MySqlCommand cmd2 = new MySqlCommand(zapytanie, connection);
                cmd2.ExecuteNonQuery();
                MySqlDataReader dataReader2 = cmd2.ExecuteReader();
                dataReader2.Read();
                //MessageBox.Show(dataReader2.GetString(1));
                events[j] = new Tuple<string, string, string>(dataReader2.GetString(0), dataReader2.GetString(1), Convert.ToString(dataReader2.GetDateOnly(2)));
                dataReader2.Close();
                i++;
                wydarzenie.Items.Add(events[j].Item1);
            }
        }

        private void wydarzenie_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            while (events[i].Item1 != wydarzenie.SelectedItem.ToString())
            {
                i++;
            }
            data.Text = events[i].Item3;
            agenda.Text = events[i].Item2;
        }

        private void typuczestnika_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = typuczestnika.SelectedIndex;
            int count = typuczestnika.Items.Count;
            for (int i = 0; i < count; i++)
            {
                if (i != index)
                {
                    typuczestnika.SetItemChecked(i, false);
                }
            }

        }

        private void jedzenie_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = jedzenie.SelectedIndex;
            int count = jedzenie.Items.Count;
            for (int i = 0; i < count; i++)
            {
                if (i != index)
                {
                    jedzenie.SetItemChecked(i, false);
                }
            }
        }

        private void b_wydarzenie_Click_1(object sender, EventArgs e)
        {
            if (wydarzenie.SelectedIndex > -1 && typuczestnika.CheckedItems.Count > 0 && jedzenie.CheckedItems.Count > 0)
            {
                string checkbox = "";
                foreach (object item in typuczestnika.CheckedItems)
                {
                    checkbox = item.ToString();
                }

                string checkbox2 = "";
                foreach (object item in jedzenie.CheckedItems)
                {
                    checkbox2 = item.ToString();
                }

                connection = new MySqlConnection(connectionString);
                connection.Open();
                string zapytanie = "select id from event where nazwa = '" + wydarzenie.SelectedItem.ToString() + "'";
                MySqlCommand cmd = new MySqlCommand(zapytanie, connection);
                cmd.ExecuteNonQuery();
                MySqlDataReader dataReader = cmd.ExecuteReader();
                dataReader.Read();
                int id = dataReader.GetInt32(0);
                dataReader.Close();
                string akt = "insert into zapisy (id_uzytkownika, id_wydarzenia, typ_uczestnictwa, wyzywienie) values (" + user_id + ", " + id + ", '" + checkbox + "', '" + checkbox2 + "')";
                MySqlCommand cmd2 = new MySqlCommand(akt, connection);
                cmd2.ExecuteNonQuery();
                MessageBox.Show("Dodano do wydarzenia");
                wydarzenie.Items.Clear();
                typuczestnika.ClearSelected();
                jedzenie.ClearSelected();
                agenda.Clear();
                data.Clear();
            }
            else
            {
                MessageBox.Show("Nie wybrano wydarzenia lub nie wybrano typu uczestnika lub jedzenia");
            }
        }

        private void b_add_user_Click(object sender, EventArgs e)
        {
            if (add_user_imie.Text != "" && add_user_nazwisko.Text != "" && add_user_login.Text != "" && add_user_haslo.Text != "" && add_user_email.Text != "" && add_user_uprawnienia.Text != "")
            {
                string query = "insert into users (imie, nazwisko, login, haslo, email, uprawnienia, data_rejestracji) values " +
                "('" + add_user_imie.Text + "', '" + add_user_nazwisko.Text + "', '" + add_user_login.Text + "', '" + add_user_haslo.Text + "', '"
                + add_user_email.Text + "', '" + add_user_uprawnienia.Text + "', '" + DateTime.Now.ToString("yyyy-MM-dd") + "')";
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Dodano u¿ytkownika");
                add_user_imie.Clear();
                add_user_nazwisko.Clear();
                add_user_login.Clear();
                add_user_haslo.Clear();
                add_user_email.Clear();
                add_user_uprawnienia.Clear();
            }
            else
            {
                MessageBox.Show("Nie wype³niono wszystkich pól");
            }           
        }

        private void b_delete_user_Click(object sender, EventArgs e)
        {
            if (delete_user_login.Text != "")
            {
                string query = "delete from users where login = " + delete_user_login.Text;
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Usuniêto u¿ytkownika");
                delete_user_login.Clear();
            }
            else
            {
                MessageBox.Show("Nie wype³niono wszystkich pól");
            }        
        }

        private void b_resetpass_Click(object sender, EventArgs e)
        {
            if (reset_login.Text!= "" && reset_password.Text != "")
            {
                string query = "update users set haslo = '" + reset_password.Text + "' where login = " + reset_login.Text;
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Zresetowano has³o");
                reset_password.Clear();
                reset_login.Clear();
            }
            else
            {
                MessageBox.Show("Nie wype³niono wszystkich pól");
            }
        }

        private void b_addevent_Click(object sender, EventArgs e)
        {
            if (add_eventname.Text !="" && add_eventagenda.Text!=""  && add_eventdate.Text!="")
            {
                string query = "insert into event (nazwa, agenda, termin) values ('" + add_eventname.Text + "', '" + add_eventagenda.Text + "', '" + add_eventdate.Text + "')";
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Dodano wydarzenie");
                add_eventname.Clear();
                add_eventagenda.Clear();
                add_eventdate.Clear();
            }
            else
            {
                MessageBox.Show("Nie wype³niono wszystkich pól");
            }
        }

        private void delete_event_list_DropDown(object sender, EventArgs e)
        {
            delete_event_list.Items.Clear();
            connection = new MySqlConnection(connectionString);
            connection.Open();
            string zapytanie = "select nazwa from event";
            MySqlCommand cmd = new MySqlCommand(zapytanie, connection);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                delete_event_list.Items.Add(dataReader.GetString(0));
            }
            dataReader.Close();
        }
        
        private void b_deletevent_Click(object sender, EventArgs e)
        {
            if(delete_event_list.SelectedIndex > -1)
            {
                string query = "delete from event where nazwa = '" + delete_event_list.Text + "'";
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Usuniêto wydarzenie");
                delete_event_list.SelectedIndex = -1;
                delete_event_list.Items.Clear();
            }
            else
            {
                MessageBox.Show("Nie wybrano wydarzenia");
            }
        }

        private void modify_event_list_DropDown(object sender, EventArgs e)
        {
            modify_event_list.Items.Clear();
            connection = new MySqlConnection(connectionString);
            connection.Open();
            string zapytanie = "select nazwa from event";
            MySqlCommand cmd = new MySqlCommand(zapytanie, connection);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                modify_event_list.Items.Add(dataReader.GetString(0));
            }
            dataReader.Close();

        }
            
        private void modify_event_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "select nazwa,agenda,termin from event where nazwa = '" + modify_event_list.Text + "'";
            connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                modify_event_nazwa.Text = dataReader.GetString(0);
                modify_event_agenda.Text = dataReader.GetString(1);
                modify_event_termin.Text = Convert.ToString(dataReader.GetDateOnly(2));
            }
        }

        private void b_modifyevent_Click(object sender, EventArgs e)
        {
            string query = "update event set nazwa = '" + modify_event_nazwa.Text + "', agenda = '" 
                + modify_event_agenda.Text + "', termin = '" + modify_event_termin.Text + "' where nazwa = '" + modify_event_list.Text + "'";

            connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Zmodyfikowano wydarzenie");
            modify_event_list.SelectedIndex = -1;
            modify_event_list.Items.Clear();
            modify_event_nazwa.Clear();
            modify_event_agenda.Clear();
            modify_event_termin.Clear();
        }

        Tuple<string, int, int>[] zapisy_lista;
        private void lista_zapisow_DropDown(object sender, EventArgs e)
        {
            lista_zapisow.Items.Clear();
            string query = "select count(*) from zapisy";
            connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            dataReader.Read();
            zapisy_lista = new Tuple<string, int, int>[dataReader.GetInt32(0)];
            dataReader.Close();
            string query2 = "select id_uzytkownika, id_wydarzenia from zapisy";
            MySqlCommand cmd2 = new MySqlCommand(query2, connection);
            cmd2.ExecuteNonQuery();
            MySqlDataReader dataReader2 = cmd2.ExecuteReader();
            int i = 0;
            while (dataReader2.Read())
            {
                zapisy_lista[i] = new Tuple<string, int, int>("Zapis nr "+(int)(i+1), dataReader2.GetInt32(0), dataReader2.GetInt32(1));
                lista_zapisow.Items.Add(zapisy_lista[i].Item1);
                i++;
            }
        }


        private void lista_zapisow_SelectedIndexChanged(object sender, EventArgs e)
        {
            zapisy_imie.ReadOnly = true;
            zapisy_nazwisko.ReadOnly = true;
            zapisy_login.ReadOnly = true;
            zapisy_event_name.ReadOnly = true;
            zapisy_uczestnictwo.ReadOnly = true;
            zapisy_jedzenie.ReadOnly = true;

            int i = lista_zapisow.SelectedIndex;
            if(i > -1)
            {
                int id_uzytkownika = zapisy_lista[i].Item2;
                int id_wydarzenia = zapisy_lista[i].Item3;
                string query = "select imie, nazwisko, login, nazwa, typ_uczestnictwa, wyzywienie from " +
                "users, event,zapisy where id_uzytkownika=users.id and id_wydarzenia=event.id and id_uzytkownika = " 
                + id_uzytkownika + " and id_wydarzenia = " + id_wydarzenia;
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    zapisy_imie.Text = dataReader.GetString(0);
                    zapisy_nazwisko.Text = dataReader.GetString(1);
                    zapisy_login.Text = dataReader.GetString(2);
                    zapisy_event_name.Text = dataReader.GetString(3);
                    zapisy_uczestnictwo.Text = dataReader.GetString(4);
                    zapisy_jedzenie.Text = dataReader.GetString(5);
                }
            }
        }

        private void accept_deny_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = accept_deny.SelectedIndex;
            int count = accept_deny.Items.Count;
            for (int i = 0; i < count; i++)
            {
                if (i != index)
                {
                    accept_deny.SetItemChecked(i, false);
                }
            }
        }

        private void b_zapisy_Click(object sender, EventArgs e)
        {
            if (lista_zapisow.SelectedIndex > -1 && accept_deny.CheckedItems.Count > 0)
            {
                int index = lista_zapisow.SelectedIndex;
                string checkbox = "";
                foreach (object item in accept_deny.CheckedItems)
                {
                    checkbox = item.ToString();
                }

                int i = lista_zapisow.SelectedIndex;
                int id_uzytkownika = zapisy_lista[i].Item2;
                int id_wydarzenia = zapisy_lista[i].Item3;
                int wybor = 0;

                string query = "update zapisy set zatwierdzenie = " + wybor + " where id_uzytkownika = " + id_uzytkownika + " and id_wydarzenia = " + id_wydarzenia;

                if (Equals(checkbox, "Zaakceptuj"))
                {
                    wybor = 1;
                }

                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Zmodyfikowano zapis");
                lista_zapisow.SelectedIndex = -1;
                lista_zapisow.Items.Clear();
                zapisy_imie.Clear();
                zapisy_nazwisko.Clear();
                zapisy_login.Clear();
                zapisy_event_name.Clear();
                zapisy_uczestnictwo.Clear();
                zapisy_jedzenie.Clear();
                
            }
            else
            {
                MessageBox.Show("Nie wybrano zapisu lub nie wybrano akceptacji");
            }

        }
   
    }
}