using System;
using System.Collections.Generic;
using UnityEngine;

namespace LUX {
    public enum DimensionType { Hell, Purgatory, Heaven }
    public class WorldGenerator : MonoBehaviour {
        [SerializeField] private Dimension[] dimensions;
        [SerializeField] private List<RoomData> rooms;
        public Dimension[] Dimensions => dimensions;
        public List<RoomData> Rooms => rooms;
        [SerializeField] private Portal[] portals;
        //[SerializeField] private TilePack[] tilePacks;
        // settings
        const int maxRoomsToTravel = 17;
        const int unkown = 10;
        const int minions = 20;        
        const int champions = 5;
        const int uniques = 2;
        const int shrines = 5;
        const int markets = 3;

        // index trackers
        int level = 0;
        public int Level => level;
        DimensionType dimension = DimensionType.Hell;
        public DimensionType CurrentDimension => dimension;

        TilePack currentTilePack;
        public TilePack CurrentTilePack => currentTilePack;

        RoomType[] roomsToAdd;
        Stack<RoomType> generatedRooms;

        private void Awake() {
            roomsToAdd = new RoomType[unkown+minions+champions+uniques+shrines+markets];            
            generatedRooms = new Stack<RoomType>();
        }        

        public void Init() {   
            AddRooms();
            ShuffleRooms();
            AddRoomsToStack();          
            GenerateRooms();
            AssignRoomsToPortals();
        }

        public void RaiseLevel() {
            level++;
        }

        public void AssignRoomsToPortals() {
            int i = 0;
            foreach(Portal p in portals) {
                p.AssignRoom(rooms[level].Portals[i]);
                i++;
            }         
        }      

        private void AddRooms() {
            int i = 0;
            for(int j = 0; j < unkown; j++) {
                roomsToAdd[i] = RoomType.Unknown;
                i++;
            }
            for(int j = 0; j < minions; j++) {
                roomsToAdd[i] = RoomType.Minions;
                i++;
            }            
            for(int j = 0; j < champions; j++) {
                roomsToAdd[i] = RoomType.Champion;
                i++;
            }
            for(int j = 0; j < uniques; j++) {
                roomsToAdd[i] = RoomType.Unique;
                i++;
            }
            for(int j = 0; j < shrines; j++) {
                roomsToAdd[i] = RoomType.Shrine;
                i++;
            }
            for(int j = 0; j < markets; j++) {
                roomsToAdd[i] = RoomType.Market;
                i++;
            }
        }

        private void ShuffleRooms() {
            // shuffle rooms array
            System.Random rng = new System.Random();
            Utilities.ShuffleArray<RoomType>(rng, roomsToAdd);
        }

        private void AddRoomsToStack() {
            // stack shuffled rooms
            foreach(RoomType r in roomsToAdd) {
                generatedRooms.Push(r);
            }
        } 

        private void GenerateRooms() {
            for(int i = 0; i < maxRoomsToTravel; i++ ) { 
                RoomData room = new RoomData();
                room.Portals = new RoomType[3] {
                    generatedRooms.Count > 3 ? generatedRooms.Pop() : RoomType.Boss,
                    generatedRooms.Count > 1 ? generatedRooms.Pop() : RoomType.Boss,
                    generatedRooms.Count > 0 ? generatedRooms.Pop() : RoomType.Boss
                };
                rooms.Add(room);
            }
        }
    }
}