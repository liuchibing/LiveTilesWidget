using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.Text.RegularExpressions;

namespace LiveTilesWidget
{
    /// <summary>
    /// һ�����Ա���̬����С����������Ӧ�õ���Ϣ,��һ����̬������������Ϣ
    /// </summary>
    public class TileDetail
    {
        /// <summary>
        /// ��ʾ�ı�ǩ����
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// ���������
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// ͼ��
        /// </summary>
        public Bitmap Icon
        {
            get;
            set;
        }
        //private Bitmap icon;

        /// <summary>
        /// ��̬����С������Id
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ��ڴ�������ʾ����֪ͨ
        /// </summary>
        public bool ShowNotification
        {
            get;
            set;
        }

        /// <summary>
        /// �����ı���ɫ��-1Ϊ�Զ���-2Ϊȫ���Զ���ɫ
        /// </summary>
        public int TileColor
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ�����֪ͨʱ��ʾ֪ͨ��ͼ��
        /// </summary>
        public bool ShowNotifIcon
        {
            get;
            set;
        }

        /// <summary>
        /// ��ǰ��̬������̬���ݵ�����
        /// </summary>
        public LiveTileType TileType
        {
            get;
            set;
        }

        /// <summary>
        /// ���ڸ��¶�̬������̬���ݵ�Rss Url
        /// </summary>
        public string RssUrl
        {
            get;
            set;
        }

        /// <summary>
        /// ���ص�ǰ������������Ӧ�õ�ͼ�굽Icon����
        /// </summary>
        /// <param name="context"></param>
        public void LoadIcon(Context context)
        {
            Icon = ((BitmapDrawable)context.PackageManager.GetActivityIcon(context.PackageManager.GetLaunchIntentForPackage(Name))).Bitmap;
        }

        /// <summary>
        /// ��ȡ������ListView����������ģ�ƴ��������ĸ
        /// </summary>
        /// <returns></returns>
        public string GetSortLetters()
        {
            // �������ʽ���ж�����ĸ�Ƿ���Ӣ����ĸ  
            Regex reg = new Regex("[A-Z]");
            
            //����ת����ƴ��
            string pinyin = CharacterParser.GetCharSpellCode(Label.Substring(0, 1));

            // �������ʽ���ж�����ĸ�Ƿ���Ӣ����ĸ  
            if (reg.IsMatch(pinyin))
            {
                return pinyin;
            }
            else
            {
                return "#";
            }
        }
    }

    /// <summary>
    /// ��̬������̬���ݵ�����
    /// </summary>
    public enum LiveTileType
    {
        /// <summary>
        /// �����մ������ã�����֪ͨʱ��ʾ֪ͨ���ݣ���������̬����
        /// </summary>
        None,
        /// <summary>
        /// ��ָ��RSSԴ���¶�̬����
        /// </summary>
        Rss
    }
}