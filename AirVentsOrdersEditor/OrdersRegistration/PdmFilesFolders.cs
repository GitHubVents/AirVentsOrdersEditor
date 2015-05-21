﻿using System;
using System.IO;
using System.Windows;
using EdmLib;

namespace OrdersRegistration
{
    public class PdmFilesFolders
    {
       
        public string OrderName { get; set; }

        public string VaultName { get; set; }

        public void CreateOrder()
        {
            MessageBox.Show(CopyAFile(CreateDistDirectory(@"E:\Tets_debag\Vents-PDM\Заказы AirVents Frameless\AV76654"), AsmTemplatePath, OrderName).ToString());
        }

        static readonly EdmVault5 _vault5 = new EdmVault5();
        
        const string AsmTemplatePath
        {
            get { return Path.Combine(_vault5.RootFolderPath, @"\Vents-PDM\Заказы AirVents Frameless\AV75 75B\Assm.sldasm"); }
        } 

        bool LoginVaultAuto()
        {
            if (_vault5.IsLoggedIn) return true;
            try
            {
                _vault5.LoginAuto(VaultName, 0);
                return true;
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.StackTrace);
                return false;
            }
        }

        
        
        string RootFolder
        {
            get { return _vault5.RootFolderPath; }
        }
        
        //string Folder(string orderName)
        //{
        //    return String.Format(@"{0}\{1}\{2} {3}B",
        //        Settings.Default.DestinationFolder,
        //        @"\Заказы AirVents Frameless",
        //        orderName);
        //}

        IEdmFolder5 CreateDistDirectory(string path)
        {
            IEdmFolder5 iEdmFolder5 = null;
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                if (!LoginVaultAuto()) return null;

                var vault2 = (IEdmVault7)_vault5;
                if (directoryInfo.Exists)
                {
                    iEdmFolder5 = vault2.GetFolderFromPath(directoryInfo.FullName); 
                }
                else
                {
                    if (directoryInfo.Parent != null)
                    {
                        var parentFolder = vault2.GetFolderFromPath(directoryInfo.Parent.FullName);
                        iEdmFolder5 = parentFolder.AddFolder(0, directoryInfo.Name);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
            return iEdmFolder5;
        }

        int CopyAFile(IEdmFolder5 destinationFolder, string destinationFilePath, string newName)
        {
            IEdmFolder5 oFolder;
            var edmFile5 = _vault5.GetFileFromPath(destinationFilePath, out oFolder);

            return destinationFolder.CopyFile(edmFile5.ID, oFolder.ID, 0, newName);
        }

    }
}
