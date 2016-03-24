using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;

namespace Lecturer.Data.Processor
{
    /// <summary>
    /// Обработка файлов формата xls/xlst
    /// </summary>
    public class ExcelFileProcessor
    {
        private string connectionString;

        private string sheetName;

        /// <summary>
        /// Название таблицы
        /// </summary>
        public string TableTitle { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="_sheetName">название таблицы</param>
        public ExcelFileProcessor(string path, string _sheetName)
        {
            //строка открытия бд (в данном случае - xls/xlst файла)
            connectionString = GetConnectionString(path, "No");
            sheetName = _sheetName;
        }       


        /// <summary>
        /// Строка подключения к файлу 
        /// </summary>
        /// <param name="FileName">Расположение файла на локальном компьютере</param>
        /// <param name="Header">заголовое</param>
        /// <returns>Строка подключения</returns>
        private string GetConnectionString(string path, string Header)
        {
            string FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();
            if (System.IO.Path.GetExtension(FileName).ToUpper() == ".XLS")
            {
                Builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                Builder.Add("Extended Properties", string.Format("Excel 8.0;IMEX=1;HDR={0};", Header));
            }
            else
            {
                Builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                Builder.Add("Extended Properties", string.Format("Excel 12.0;IMEX=1;HDR={0};", Header));
            }

            Builder.DataSource = FileName;

            return Builder.ConnectionString;
        }


        /// <summary>
        /// Получаем таблицу, которая соответствует выбранной
        /// институту, специальности, курсу и семестру
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="semester">семестр</param>
        /// <returns></returns>
        private DataSet GetDataSet()
        {
            DataSet ds = new DataSet();

            //пытаемся открыть и обработать файл
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    //подключаемся к источнику данных
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;

                    // получаем все листы 
                    DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    // Проходим по всем листам
                    foreach (DataRow dr in dtSheet.Rows)
                    {
                        string sheet = dr["TABLE_NAME"].ToString();
                        if (sheet.Contains(sheetName))
                        {
                            // получаем все строки
                            cmd.CommandText = "SELECT * FROM [" + sheet + "]";

                            //создаем новую таблицу
                            DataTable dt = new DataTable();
                            dt.TableName = sheet;
                            try
                            {
                                //заполняем таблицу
                                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                da.Fill(dt);

                                ds.Tables.Add(dt);

                                break;
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        else continue;
                    }

                    //закрываем соединение
                    cmd = null;
                    conn.Close();
                }
                return ds;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Список дисциплин
        /// </summary>
        /// <returns>список дисциплин</returns>
        public List<Subject> FillSource()
        {
            //хранилище
            List<Subject> subj = new List<Subject>();
            DataSet ds = GetDataSet();
            try
            {
                //единственная таблица со списком дисциплин
                DataTable table = ds.Tables[0];

                int counter = table.Rows[0].ItemArray.Count();
                int teacherName = 0, subjectName = 0;

                //ищем столбцы, в которых указаны имена преподавателей и названия дисциплин
                for(int i = 0; i < counter; i++)
                {
                    if (table.Rows[0][i].ToString().ToLower() == "викладач")
                        teacherName = i;
                    else if (table.Rows[0][i].ToString().ToLower() == "спеціальність")
                        subjectName = i;
                }

                //имя таблицы - код группы
                TableTitle = table.Rows[subjectName+2][1].ToString();


                counter = ds.Tables[0].Rows.Count;
                for (int i = subjectName; i < counter; i++)
                {
                    DataRow row = table.Rows[i];
                    if (row[1].ToString() != "")
                    {
                        //проверяем, есть ли теоретическая часть
                        string hours = (row[2].ToString() == "лк") ? row[3].ToString() : "0";
                        if (hours != "0")
                        {
                            subj.Add(new Subject
                            {
                                Name = row[1].ToString(),
                                Teacher = "Викладач: " + row[teacherName].ToString(),
                                Hours = hours,
                                ID = row[2].ToString()
                            });
                        }
                    }
                }

                return subj;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        

        
    }
}
