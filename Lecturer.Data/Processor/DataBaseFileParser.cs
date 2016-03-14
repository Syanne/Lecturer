using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;

namespace Lecturer.Data.Processor
{
    public class DataBaseFileParser
    {
        private string ConnectionString { get; set; }


        private string sheetName;


        public DataBaseFileParser(string path, string _semester)
        {
            //строка открытия бд (в данном случае - xls/xlst файла)
            ConnectionString = GetConnectionString(path, "No");
            sheetName = _semester;
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
                using (OleDbConnection conn = new OleDbConnection(ConnectionString))
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
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void FillSource(List<Subject> source) 
        {
            DataSet ds = GetDataSet();
            try
            {
                int counter = ds.Tables[0].Rows.Count;
                DataTable table = ds.Tables[0];
                for (int i = 1; i < counter; i++)
                {
                    DataRow row = table.Rows[i];
                    source.Add(new Subject
                    {
                        Name = row[0].ToString(),
                        Hours = row[1].ToString(),
                        ID = row[2].ToString()
                    });
                }
                
            }
            catch
            {
                source = null;
            }
        }     
    }
}
