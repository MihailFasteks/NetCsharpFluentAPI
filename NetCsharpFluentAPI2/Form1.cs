using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
namespace NetCsharpFluentAPI2
{

    public partial class Form1 : Form
    {
        public string currentUser {  get; set; }
        public Form1(string usLog)
        {
            InitializeComponent();


            comboBox1.Items.Add("Name");
            comboBox1.Items.Add("Last Name");
            comboBox1.Items.Add("Job");
            currentUser = usLog;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            using (FluentContext db = new FluentContext())
            {
                Job j1 = new Job { Name = "Manager" };
                Job j2 = new Job { Name = "Director" };
                Job j3 = new Job { Name = "Cleaner" };

                db.Jobs.AddRange(j1, j2, j3);
                await db.SaveChangesAsync();

                MessageBox.Show("ДобавленоЙ!");

                Employee em1 = new Employee { FirstName = "Иван", LastName = "Иванов", Job = j1 };
                Employee em2 = new Employee { FirstName = "Петр", LastName = "Петров", Job = j2 };
                Employee em3 = new Employee { FirstName = "Алексей", LastName = "Алексеев", Job = j3 };

                db.Employees.AddRange(em1, em2, em3);
                await db.SaveChangesAsync();

                var employees = await (from em in db.Employees
                                       join j in db.Jobs on em.Job.Id equals j.Id
                                       select new
                                       {
                                           Name = em.FirstName,
                                           LastName = em.LastName,
                                           Job = j.Name
                                       }).ToListAsync();

                dataGridView1.DataSource = employees;
            }
        }
        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show($"Фильтр: {comboBox1.SelectedItem}, Введено: {textBox1.Text}");
            if (string.IsNullOrWhiteSpace(textBox1.Text)) return;

            using (var context = new FluentContext())
            {
                var query = from em in context.Employees
                            join j in context.Jobs on em.Job.Id equals j.Id
                            select new
                            {
                                Name = em.FirstName,
                                LastName = em.LastName,
                                Job = j.Name
                            };

                if (comboBox1.SelectedIndex == 0)
                {
                    query = query.Where(em => em.Name.ToLower().Trim() == textBox1.Text.ToLower().Trim());
                   
                    
                }
                  
                else if (comboBox1.SelectedIndex == 1)
                {
                    query = query.Where(em => em.LastName.ToLower().Trim() == textBox1.Text.ToLower().Trim());
                }
                   
                else if (comboBox1.SelectedIndex == 2)
                {
                    query = query.Where(em => em.Job == textBox1.Text);
                }
                  

                var result = await query.ToListAsync();
                dataGridView1.DataSource = result;
                WriteToJsonFile(new { CurrentUser = currentUser, Message = $"Пользователь выбрал фильтр по {comboBox1.SelectedText}!" });

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox4.Text != "")
            {
                using (FluentContext db = new FluentContext())
                {
                    Job job = db.Jobs.FirstOrDefault(j => j.Name.ToLower() == textBox5.Text.ToLower());
                    Employee em = new Employee { FirstName = textBox2.Text, LastName = textBox3.Text, Age = int.Parse(textBox4.Text), Job = job };


                    db.Employees.Add(em);
                    db.SaveChanges();

                    var employees = from empl in db.Employees
                                    join j in db.Jobs on empl.Job.Id equals j.Id
                                    select new
                                    {
                                        Name = empl.FirstName,
                                        LastName = empl.LastName,
                                        Job = j.Name
                                    };

                    dataGridView1.DataSource = employees.ToList();
                    MessageBox.Show("Сотрудник добавлен!");
                    WriteToJsonFile(new { CurrentUser = currentUser, Message = "Пользователь добавил сотрудника!" }); ;
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";

                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FluentContext db = new FluentContext())
            {

                var selectedRow = dataGridView1.CurrentRow;
                var firstName = selectedRow.Cells["Name"].Value;
                var lastName = selectedRow.Cells["LastName"].Value;
              
                var jobName = selectedRow.Cells["Job"].Value;

                Job job = db.Jobs.FirstOrDefault(j => j.Name == jobName);

                Employee selectedEmloyee = db.Employees.FirstOrDefault(empl => empl.FirstName == firstName && empl.LastName == lastName && empl.Job == job);
                db.Employees.Remove(selectedEmloyee);
                db.SaveChanges();

                var employees = from empl in db.Employees
                                join j in db.Jobs on empl.Job.Id equals j.Id
                                select new
                                {
                                    Name = empl.FirstName,
                                    LastName = empl.LastName,
                                    Job = j.Name
                                };

                dataGridView1.DataSource = employees.ToList();
                MessageBox.Show("Сотрудник удален!");
                WriteToJsonFile(new { CurrentUser = currentUser, Message = "Пользователь удалил сотрудника!" });
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FluentContext db = new FluentContext())
            {

                var selectedRow = dataGridView1.CurrentRow;
                var firstName = selectedRow.Cells["Name"].Value;
                var lastName = selectedRow.Cells["LastName"].Value;
          
                var jobName = selectedRow.Cells["Job"].Value;



                Job job = db.Jobs.FirstOrDefault(j => j.Name == jobName);


                Employee selectedEmloyee = db.Employees.FirstOrDefault(empl => empl.FirstName == firstName && empl.LastName == lastName && empl.Job == job);
                Job job1=db.Jobs.FirstOrDefault(j => j.Name == textBox5.Text);
                selectedEmloyee.FirstName = textBox2.Text;
                selectedEmloyee.LastName = textBox3.Text;
                
                if (job1!=null)
                {
                    selectedEmloyee.Job=job1;
                }
                if (textBox2.Text != "" && textBox3.Text != ""  && job1 != null)
                {
                    db.SaveChanges();

                    var employees = from empl in db.Employees
                                    join j in db.Jobs on empl.Job.Id equals j.Id
                                    select new
                                    {
                                        Name = empl.FirstName,
                                        LastName = empl.LastName,
                                        Job = j.Name
                                    };

                    dataGridView1.DataSource = employees.ToList();
                }
                else
                {
                    return;
                }
             
                MessageBox.Show("Сотрудник изменен!");
                WriteToJsonFile(new { CurrentUser = currentUser, Message = "Пользователь изменил сотрудника!" });
                textBox2.Text = "";
                textBox3.Text = "";
           
                textBox5.Text = "";

            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var selectedRow = dataGridView1.CurrentRow;

            var firstName = selectedRow.Cells["Name"].Value;
            var lastName = selectedRow.Cells["LastName"].Value;
          
            var jobName = selectedRow.Cells["Job"].Value;

            textBox2.Text = (string)firstName;
            textBox3.Text = (string)lastName;
           
            textBox5.Text = (string)jobName;


        }
        private void WriteToJsonFile(object data)
        {
            string filePath = "C:\\Users\\fasta\\Desktop\\PROG\\C#\\2 semestr\\HW\\NetCsharpFluentAPI2\\NetCsharpFluentAPI2\\json1.json";

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            using (StreamWriter sw = new StreamWriter(filePath, append: true))
            {
                string json = JsonSerializer.Serialize(data, options);
                sw.WriteLine(json);
            }
        }
    }
}
