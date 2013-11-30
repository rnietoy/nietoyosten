//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using FileHelpers;

//namespace ImportNietoData
//{
//    [DelimitedRecord(";")]
//    public class Article
//    {
//        [FieldQuoted()]
//        public int id;
//        [FieldQuoted()]
//        public string title;
//        [FieldQuoted()]
//        public string title_alias;
//        [FieldQuoted()]
//        public string introtext;
//        [FieldQuoted()]
//        public string fulltext;
//        [FieldQuoted()]
//        [FieldNullValue((byte)0)]
//        public byte state;
//        [FieldQuoted()]
//        public int sectionid;
//        [FieldQuoted()]
//        public int mask;
//        [FieldQuoted()]
//        public int catid;
//        [FieldQuoted()]
//        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
//        public DateTime created;
//        [FieldQuoted()]
//        public int created_by;
//        [FieldQuoted()]
//        public string created_by_alias;
//        [FieldQuoted()]
//        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
//        public DateTime modified;
//        [FieldQuoted()]
//        public int modified_by;
//        [FieldQuoted()]
//        public int checked_out;
//        [FieldQuoted()]
//        public string checked_out_time;
//        [FieldQuoted()]
//        public string publish_up;
//        [FieldQuoted()]
//        public string publish_down;
//        [FieldQuoted()]
//        public string images;
//        [FieldQuoted()]
//        public string urls;
//        [FieldQuoted()]
//        public string attribs;
//        [FieldQuoted()]
//        public int version;
//        [FieldQuoted()]
//        public int parentid;
//        [FieldQuoted()]
//        public int ordering;
//        [FieldQuoted()]
//        public string metakey;
//        [FieldQuoted()]
//        public string metadesc;
//        [FieldQuoted()]
//        public int access;
//        [FieldQuoted()]
//        public int hits;
//    }
//}
