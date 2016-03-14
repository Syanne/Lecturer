using Microsoft.Vbe.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Lecturer.Data.DataProcessor
{
    public class XMLFileProcessor
    {
        private string Path { get; set; }
        private XDocument PersonalData { get; set; }

        public XMLFileProcessor(string pathString)
        {
            Path = pathString;

            try
            {
                PersonalData = XDocument.Load(Path);
            }
            catch
            {
                PersonalData = new XDocument();
            }
        }
        
        /// <summary>
        /// Saves changed personal data
        /// </summary>
        public void SaveDocument()
        {
            PersonalData.Save(Path, SaveOptions.OmitDuplicateNamespaces);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        public void CreateSettingsFile(Dictionary<string, string> dictionary)
        {
            try
            {
                //root
                PersonalData = new XDocument(new XElement("data", PersonalData.Root));
                
                //first parent
                using (XmlWriter writer = PersonalData.Root.CreateWriter())
                {
                    writer.WriteStartElement("userdata");
                    writer.WriteString("");
                    writer.WriteEndElement();
                }

                //userdata
                using (XmlWriter innerWriter = PersonalData.Root.Element("userdata").CreateWriter())
                {
                    for (int i = 0; i < dictionary.Count; i++)
                    {
                        innerWriter.WriteStartElement(dictionary.Keys.ElementAt(i));
                        innerWriter.WriteString(dictionary.Values.ElementAt(i));
                        innerWriter.WriteEndElement();
                    }                    
                }                

                //save changes
                SaveDocument();
            }
            catch (Exception e)
            {
                MyMessage(e.Message);
            }
        }

        public string ReadValue(string parent, string key)
        {
            try
            {
                return PersonalData.Root.Element(parent).Element(key).Value;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// Change note
        /// </summary>
        /// <param name="oldName">old text</param>
        /// <param name="newName">changed text</param>
        /// <param name="day">selected day</param>
        /// <param name="month">selected month</param>
        /// <param name="year">selected year (or 0)</param> 
        public void ChangePersonal(string oldName, string newName, string day, string month, string year)
        {
            try
            {
                PersonalData.Root.Descendants("holidays").Descendants("persDate").
                    Where(p => (p.Attribute("name").Value == oldName)).
                    Where(p => (p.Attribute("date").Value == day)).
                    Where(p => (p.Attribute("month").Value == month)).FirstOrDefault().
                    ReplaceAttributes(new XAttribute("name", newName),
                    new XAttribute("date", day),
                    new XAttribute("month", month),
                    new XAttribute("year", year));

                //save changes
                SaveDocument();
            }
            catch (Exception e)
            {
                MyMessage(e.Message);
            }
        }

        /// <summary>
        /// Remove note
        /// </summary>
        /// <param name="startText"></param>
        public void RemoveHoliday(string name, string day, string month, string year)
        {
            try
            {
                //try to load PersData.xml            
                PersonalData.Root.Descendants("holidays").
                    Descendants("persDate").
                    Where(p => (p.Attribute("name").Value == name)).
                    Where(p => (p.Attribute("date").Value == day)).
                    Where(p => (p.Attribute("month").Value == month)).
                    Where(p => (p.Attribute("year").Value == year)).
                        Remove();

                //save changes
                SaveDocument();
            }
            catch (Exception e)
            {
                MyMessage(e.Message);
            }
        }

        private void MyMessage(string text)
        {
            //var dial = new MessageDialog(text);

            //dial.Commands.Add(new UICommand("OK"));
            //var command = await dial.ShowAsync();
        }
    }

}