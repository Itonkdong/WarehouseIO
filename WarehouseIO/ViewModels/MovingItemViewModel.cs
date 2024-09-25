﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class MovingItemViewModel
    {

        public const string DEFAULT_IMAGE_URL = "/Content/Images/default-item-picture.png";

        public int Id { get; set; } 
        public bool IncludeAll { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public int Size { get; set; }

        public ItemType Type { get; set; }

        public long AvailableAmount { get; set; }
        public int TransferAmount { get; set; }

        public double EstPrice { get; set; }

        public string ImageUrlRender => this.ImageUrl ?? DEFAULT_IMAGE_URL;


        public MovingItemViewModel()
        {
            
        }

        public MovingItemViewModel(Item item)
        {
            this.Id= item.Id;
            this.Name = item.Name;
            this.Size = item.Size;
            this.Type = item.Type;
            this.ImageUrl = item.ImageUrl;
            this.AvailableAmount = item.Amount;
            this.EstPrice = item.EstPrice;
        }
    }
}