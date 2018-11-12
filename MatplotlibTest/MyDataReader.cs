using DevExpress.Xpo;
using ENVIS.Model;
using KMB.Structures;
using KMB.Structures.SMP;
using KMB.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatplotlibTest
{
    public class MyDataReader
    {
        private UniRowCollection rc;
        private IEnumerator<KmbStructure> rcen;
        private UniDataStoreFile uds;
        private SmpMeasNameDB record;
        private int cntr = 0;
        private string strInfo;
        private Type lastType = null;
        private List<PropertyDescEx> props;
        private byte arch = 0;
        private bool pressed = true;

        public Dictionary<DateTime, string> values;

        public MyDataReader()
        {
            var tag1 = (byte)SmpArchiveTypes.ARCH_MAIN;
            var tag2 = (byte)SmpArchiveTypes.ARCH_ELMER;
            var tag3 = (byte)SmpArchiveTypes.ARCH_PQMAIN;
            Hyper.ComponentModel.HyperTypeDescriptionProvider.Add(typeof(XPLiteObject));
            ENVIS.Model.DataLayerHelper.DefaultSessionInit();

            values = new Dictionary<DateTime, string>();
        }

        //load cea file
        public void LoadCea(string file)
        {
            if (uds != null)
            {
                uds.Close();//close file if some is opened
                uds = null;
            }
            uds = new UniDataStoreFile(file);//cereate instance of UniDataStoreFile and initialize file
            SmpMeasNameDB[] records = uds.OList.GetRecord((SmpObjectDB)null);//get array of all records in file
            record = records[0];
            LoadArchive();
            //btnCfg.Enabled = true;
        }
        //load archive from datasource
        public void LoadArchive()
        {
            if (uds == null) return;
            if (rc != null) rc.Dispose();
            var archD = new ArchDescriptionDB(arch, record.identifyDB);
            rc = new UniRowCollection(uds, record, archD, null, null, 0, false);//create collection with archive rows
            strInfo = archD.Name + " {0}/" + rc.Count;
            //lblInfo.Text = string.Format(strInfo, 0);
            rcen = rc.GetEnumerator();
            bool n = rcen.MoveNext();
            if (n)
            {
                cntr = 1;
                lastType = null;
                LoadNext();
            }
            else
            {
                rc.Dispose();
            }
        }

        //read next row from archive
        public UniArchiveBase LoadNext()
        {
            string str = "P_avg_3P_C";
            UniArchiveBase row = rcen.Current as UniArchiveBase;
            if (row == null)
            {
                return null;
            }

            //lblInfo.Text = string.Format(strInfo, cntr++);
            Type t = row.GetType();
            if (t != lastType)
            {
                //string str = tbProps.Text.Trim();
                //props = PropertyDescEx.GetProperties(t, null); //get all properties of type t 

                props = new List<PropertyDescEx>();
                string[] pstr = str.Split(';');
                PropertyDescEx prop;
                foreach (var item in pstr)
                {
                    prop = PropertyDescEx.GetByName(item, t);//get property of ty by name
                    if (prop != null) props.Add(prop);
                }
                ;
                StringBuilder sb = new StringBuilder();

                PropertyDescEx item2 = props[0];
                sb.Append(row.TimeLocal.ToString() + "  ");
                sb.Append(item2.GetName() + "\t " + item2.GetUserName(false, false) + " = " + item2.GetValue(row).ToString() + " " + item2.QuantityAtribute.unit);
                //Console.WriteLine(sb.ToString());

                if (values.TryGetValue(row.TimeLocal, out var selected) == false)
                {
                    values.Add(row.TimeLocal, item2.GetValue(row).ToString());
                }

                bool n = rcen.MoveNext();
                if (!n)
                {
                    rc.Dispose();
                }

            }
            return row;
        }
    }
}
