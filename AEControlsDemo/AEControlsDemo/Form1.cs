using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.DataSourcesGDB;

namespace AEControlsDemo
{
    public partial class Form1 : Form
    {
        private string m_Path = Application.StartupPath + @"\Data";


        public Form1()
        {
            InitializeComponent();
        }
        #region 加载mxd地图文档
        private void 加载mxd地图文档toolStripLabel1_Click(object sender, EventArgs e)
        {
            //方法一：
            //loadMapDoc1();//调用MapControl控件的LoadMxFile方法

            //方法二：
            loadMapDoc2();
        }
        /// <summary>
        /// 方法二：运用MapDocument对象中的Open方法的函数加载mxd文档
        /// </summary>
        private void loadMapDoc2()
        {
            IMapDocument mapDocument = new MapDocumentClass();
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "打开地图文档";
                ofd.Filter = "map documents(*.mxd)|*.mxd";
                if(ofd.ShowDialog()==DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    //filePath——地图文档的路径, ""——赋予默认密码
                    mapDocument.Open(filePath, "");
                    for (int i = 0; i < mapDocument.MapCount; i++)
                    {
                        //通过get_Map(i)方法逐个加载
                        axMapControl1.Map = mapDocument.get_Map(i);
                    }
                    axMapControl1.Refresh();
                }
                else
                {
                    mapDocument = null;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
     
        /// <summary>
        /// 方法一：运用LoadMxFile方法的函数参数加载地图文档
        /// </summary>
        private void loadMapAccDoc1()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开地图文档";
            ofd.Filter = "map documents(*.mxd)|*.mxd";
            ofd.InitialDirectory = m_Path;
            //判断, 如果对话框结构不为OK, 退出函数体
            DialogResult DR = ofd.ShowDialog();
            if (DR != DialogResult.OK)
                return;
            string filePath = ofd.FileName;
            if (axMapControl1.CheckMxFile(filePath))
            {
                //设置axMapControl控制鼠标指针图标选项为沙漏光标
                axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerArrowHourglass;
                //三个参数（filePath——文件路径、0——地址名称或索引、Type.Missing——通过反射进行调用获取参数的默认值）
                axMapControl1.LoadMxFile(filePath, 0, Type.Missing);
                //定义axMapControl控制鼠标指针图标为默认箭头
                axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerDefault;
                axMapControl1.Extent = axMapControl1.FullExtent;
            }
            else
            {
                MessageBox.Show(filePath + "不是有效的地图文档");
            }
        }
        #endregion

        #region 打开Shp文件
        private void 打开Shp文件toolStripLabel1_Click(object sender, EventArgs e)
        {
            //方法一：
            ///addShapeFile1();
            //方法二：
            addShapeFile2();
        }
        /// <summary>
        /// 方法二：使用axMapControl1对象的AddLayer方法加载ShapeFile文件
        /// </summary>
        private void addShapeFile2()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开shp文件";
            ofd.Filter = "shp layer(*.shp)|*.shp";
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                string file = ofd.FileName;
                int index = 0;
                //获取最后一个“\\”时的索引位置
                index = file.LastIndexOf("\\");
                //获得shp文件的路径
                string filePath = file.Substring(0, index);
                //获得shp文件名
                string fileName = file.Substring(index + 1, file.Length - (index + 1));
                //由工作空间工厂创建shp工作空间工厂类
                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                //打开shp文件的路径目录, 并强转赋予要素工作空间
                IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(filePath,0) as IFeatureWorkspace;
                //创建要素图层
                IFeatureLayer pFLayer = new FeatureLayerClass();
                //打开文件名
                pFLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(fileName);
                //定义pFLayer的别名
                pFLayer.Name = pFLayer.FeatureClass.AliasName;
                //调用AddLayer方法添加shp图层
                this.axMapControl1.AddLayer(pFLayer as ILayer);
                //axMapControl控件刷新
                this.axMapControl1.Refresh();
            }
        }

        /// <summary>
        /// 方法一：使用axMapControl1对象的AddShapeFile方法加载ShapeFile文件
        /// </summary>
        private void addShapeFile1()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开图层文件";
            ofd.Filter = "map documents(*.shp)|*.shp";
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                //FileInfo类提供创建、复制、删除、移动和打开文件的实例方法
                FileInfo fileInfo = new FileInfo(ofd.FileName);
                //获取父目录并强制转换成字符型
                String path = fileInfo.Directory.ToString();
                //获取文件名
                String fileName = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf("."));
                try
                {
                    //path——为shp文件的路径目录,fileName——不带后缀的文件名
                    axMapControl1.AddShapeFile(path, fileName);
                }
                catch(Exception e)
                {
                    MessageBox.Show("添加图层失败！！！"+e.ToString());
                }
            }
        }
        #endregion

        #region 打开个人数据库中的要素
        private void 打开个人数据库中的要素toolStripLabel2_Click(object sender, EventArgs e)
        {

            //方法：
            AddMDBFile();
        }
        /// <summary>
        /// 方法：使用工作空间打开一个Access库中的一个要素类
        /// </summary>
        private void AddMDBFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开个人数据库";
            ofd.Filter = "Personal GDB(*.mdb)|*.mdb";
            ofd.InitialDirectory = m_Path;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            string DBPath = ofd.FileName;
            OpenWorkspaceFromFileAccess("广西师范学院平面图", DBPath);
        }
        /// <summary>
        /// 方法补充：使用工作空间打开一个Access库中的一个要素类
        /// </summary>
        /// <param name="clsName">文件名</param>
        /// <param name="DBPath">文件路径</param>
        private void OpenWorkspaceFromFileAccess(string clsName, string DBPath)
        {
            //排除没有打开指定数据的情况
            if(DBPath!=m_Path+"\\Access.mdb")
            {
                MessageBox.Show("请打开指定数据库！");
                return;
            }
            //新建一个Access的工作空间工厂
            IWorkspaceFactory pWorkspaceFactory = new AccessWorkspaceFactoryClass();
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(DBPath, 0);
            IFeatureWorkspace pAccessWorkspace=pWorkspace as IFeatureWorkspace;
            IFeatureClass pFeatureClass = pAccessWorkspace.OpenFeatureClass(clsName);
            IFeatureLayer pFLayer = new FeatureLayerClass();
            pFLayer.FeatureClass = pFeatureClass;
            pFLayer.Name = clsName;
            this.axMapControl1.AddLayer(pFLayer);
            this.axMapControl1.Refresh();
        }
        #endregion

        #region 添加TIN数据
        private void 添加TIN数据toolStripLabel1_Click(object sender, EventArgs e)
        {
            //方法：
            AddTinFile();
        }
        /// <summary>
        /// 加载栅格图层
        /// </summary>
        private void AddTinFile()
        {
            this.Cursor = Cursors.WaitCursor;
            IWorkspaceFactory pWorkspaceFactory = new TinWorkspaceFactoryClass();

            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(m_Path, 0);
            ITinWorkspace pTinWorkspace = pWorkspace as ITinWorkspace;
            //声明一个pTin变量, 存储所打开的"tin"
            ITin pTin = pTinWorkspace.OpenTin("tin");
            //将TIN变为TIN图层
            ITinLayer pTinLayer = new TinLayerClass();
            pTinLayer.Dataset = pTin;
            pTinLayer.Name = "TIN";
            //也可以用三维空间AxSceneControl加载
            //axSceneControl1.Scene.AddLayer(pTinLayer, true);
            this.axMapControl1.AddLayer(pTinLayer);
            this.Cursor = Cursors.Default;

        }

        #endregion
    }
}
