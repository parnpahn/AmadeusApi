﻿using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationApi.Models
{
    public class Reservation
    {



        //Is required for mapping the Common Language Runtime(CLR) object to the MongoDB collection.
        // Is annotated with[BsonId] to designate this property as the document's primary key.
        // Is annotated with[BsonRepresentation(BsonType.ObjectId)] to allow passing the parameter 
        //    as type string instead of an ObjectId structure. Mongo handles the conversion from string to ObjectId.
        // The BookName property is annotated with the[BsonElement] attribute. The attribute's 
        //    value of Name represents the property name in the MongoDB collection.

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        [JsonProperty("Name")]
        [BsonElement("Name")]
        public string Name { get; set; }

        public decimal Price { get; set; }
        public string RoomId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }



    }
}