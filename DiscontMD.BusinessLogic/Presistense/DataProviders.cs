﻿using DiscontMD.BusinessLogic.DomainModel;
using DiscontMD.BusinessLogic.Presistense.MSSQL;

namespace DiscontMD.BusinessLogic.Presistense
{
    public class DataProviders
    {
        public DataProviders()
        {
            Commands = GetDataProvider<CommandDTO>();
            Users = GetDataProvider<User>();
            Stores = GetDataProvider<Store>();
            Cards = GetDataProvider<Card>();
            InitData();
        }

        public IDataProvider<CommandDTO> Commands { get; private set; }
        public IDataProvider<User> Users { get; private set; }
        public IDataProvider<Store> Stores { get; private set; }
        public IDataProvider<Card> Cards { get; private set; }

        private void InitData()
        {
        }

        private IDataProvider<T> GetDataProvider<T>() where T : DomainObject, new()
        {
            return new MSSqlDataProvider<T>().Init();
        }
    }
}