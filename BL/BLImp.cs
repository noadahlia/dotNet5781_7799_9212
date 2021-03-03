﻿using System;
using System.Collections.Generic;
using System.Linq;
using DalApi;
using BLAPI;
using System.Threading;
using BO;

namespace BL
{
    //in this  class we'll implement the IBL CRUD functions
    class BLImp : IBL
    {
        IDAL dal = DalFactory.GetDal(); //new instance of the Dal layer

        //Probleme dans copyBy
        #region Bus
        BO.Bus busDoBoAdapter(DO.Bus busDO)
        {
            BO.Bus busBO = new BO.Bus();
            int id = busDO.LicenseNum;
            try
            {
                busDO = dal.GetBus(id);
            }
            catch (DO.BadBusIdException ex)
            {
                throw new BO.BadBusIdException("Bus ID doesn't exist", ex);
            }
          
            busDO.CopyPropertiesTo(busBO);          

            return busBO;
        }

        // Je vais verifier si y a 8 chiffres en supposant que les nouveaux bus seront recents et non d'avant 2018
        // A noter qu'au moment de l'affichage dans le PL essayer de mettre au format xxx-xx-xxx
        public void AddBus(Bus bus)
        {
            DO.Bus busDO = new DO.Bus();
            bus.CopyPropertiesTo(busDO);
            try
            {
                if (bus.LicenseNum.ToString().Length == 8)
                    dal.AddBus(busDO);
                else throw new Exception("License Number must be 8 digits");
            }
            catch(DO.BadBusIdException ex)
            {
                throw new BO.BadBusIdException("This bus already exists", ex);
            }

        } //pas totatelment finie //???

        public Bus GetBus(int id)
        {
            DO.Bus busDO;
            try
            {
                busDO = dal.GetBus(id);
            }
            catch (DO.BadBusIdException ex)
            {
                throw new BO.BadBusIdException("This bus doesn't exist",ex);
            }
            return busDoBoAdapter(busDO);
        }

        public void DeleteBus(int id)
        {
            try
            {
                dal.RemoveBus(id);
            }
            catch (DO.BadBusIdException ex)
            {
                throw new BO.BadBusIdException("Bus License number doesn't exist", ex);
            }
        }

        public IEnumerable<Bus> GetAllBuses()
        {
            return from busDO in dal.GetAllBuses()
                   orderby busDO.LicenseNum
                   select busDoBoAdapter(busDO);
        }

        public IEnumerable<Bus> GetBusesBy(Predicate<Bus> predicate)// Ils ont pas ete memamesh mais a completer si on le veut 
        {
            return from bus in dal.GetAllBuses()
                   where predicate(busDoBoAdapter(bus))
                   select busDoBoAdapter(bus);
        }
        public void RefuelBus(BO.Bus bus)
        {
            DO.Bus busDO = new DO.Bus();
            bus.CopyPropertiesTo(busDO);
            try
            {
                dal.RefuelBus(busDO);
            }
            catch (DO.BadBusIdException ex)
            {
                throw new BO.BadBusIdException("This bus doesn't exist", ex);
            }
        }
        public void RefreshBus(Bus bus)
        {
            DO.Bus busDO = new DO.Bus();
            bus.CopyPropertiesTo(busDO);
            try
            {
                dal.RefreshBus(busDO);
            }
            catch (DO.BadBusIdException ex)
            {
                throw new BO.BadBusIdException("This bus doesn't exist", ex);
            }
        }
        public void UpdateBusInfos(BO.Bus bus)
        {
            DO.Bus busDO = new DO.Bus();
            bus.CopyPropertiesTo(busDO);
            try
            {
                dal.UpdateBus(busDO);
            }
            catch (DO.BadBusIdException ex)
            {
                throw new BO.BadBusIdException("This bus doesn't exist", ex);
            }
        }


        #endregion

        #region Line
        BO.Line LineDoBoAdapter(DO.Line lineDO)
        {
            BO.Line lineBO = new BO.Line();
            int id = lineDO.Id;
            try
            {
                lineDO = dal.GetLine(id);
            }
            catch (DO.BadLineIdException ex)
            {
                throw new BO.BadLineIdException("Line ID doesn't exist", ex);
            }

            lineDO.CopyPropertiesTo(lineBO);

            lineBO.ListOfStations = from ls in dal.GetLineStation(ls => ls.LineId == id&&ls.IsActive)
                                    let stat = dal.GetStation(ls.Station)
                                    select stat.CopyToLineStation(ls);
           

            return lineBO;
        }
        public void AddLine(Line line)
        {
            DO.Line lineDO = new DO.Line();
            line.CopyPropertiesTo(lineDO);
            try
            {
                dal.AddLine(lineDO);
                line.ListOfStations= from ls in dal.GetLineStation(ls => ls.LineId == line.Id && ls.IsActive)
                                     let stat = dal.GetStation(ls.Station)
                                     select stat.CopyToLineStation(ls);
                //adds all LineStations where the LineId == to the new line we want to add, to the ListOfStation of the new line

            }
            catch (DO.BadLineIdException ex)
            {
                throw new BO.BadLineIdException("This line already exists", ex);
            }
        }

        public void DeleteLine(int id)
        {
            try
            {
                dal.DeleteLineFromAllStations(id);
                dal.RemoveLine(id);

            }
            catch (DO.BadLineIdException ex)
            {
                throw new BO.BadLineIdException("This line doesn't exist", ex);
            }
            catch (DO.BadLineIdStationIDException ex)
            {
                throw new BO.BadLineIdStationIDException("This line-station doesn't exist", ex);
            }
        }
        public IEnumerable<Line> GetAllLines()
        {
            return from lineDO in dal.GetAllLine()
                   orderby lineDO.Id
                   select LineDoBoAdapter(lineDO);
        }
        public Line GetLine(int id)
        {
            DO.Line lineDO;
            try
            {
                lineDO = dal.GetLine(id);
            }
            catch (DO.BadLineIdException ex)
            {
                throw new BO.BadLineIdException("This line doesn't exist", ex);
            }
            return LineDoBoAdapter(lineDO);
        }
        public IEnumerable<Line> GetLinesBy(Predicate<Line> predicate )// a completer ils l ont laisse vide efrat et dan 
        {
            return from line in dal.GetAllLines()
                   where predicate(LineDoBoAdapter(line))
                   select LineDoBoAdapter(line);//inadequat

        }
        public void UpdateLineInfos(BO.Line line)
        {
            DO.Line lineDO = new DO.Line();
            line.CopyPropertiesTo(lineDO);
            try
            {
                dal.UpdateLine(lineDO);
                line = LineDoBoAdapter(lineDO);
            }
            catch (DO.BadLineIdException ex)
            {
                throw new BO.BadLineIdException("This line doesn't exist", ex);
            }
        }

        #endregion

        #region Station
        BO.Station StationDoBoAdapter(DO.Station stationDO)
        {
            BO.Station stationBO = new BO.Station();
            int code = stationDO.Code;
            try
            {
                stationDO = dal.GetStation(code);
            }
            catch (DO.BadLineIdException ex)
            {
                throw new BO.BadLineIdException("Station code doesn't exist", ex);
            }

            stationDO.CopyPropertiesTo(stationBO);

            stationBO.ListOfLines = from linsta in dal.GetLineStation(ls => ls.Station == code && ls.IsActive)
                                    let line = dal.GetLine(linsta.LineId)
                                    select LineDoBoAdapter(line);

            return stationBO;
        }

        public void AddStation(Station station)
        {
            DO.Station stationDO = new DO.Station();
            station.CopyPropertiesTo(stationDO);
            try
            {
                dal.AddStation(stationDO);

            }
            catch (DO.BadStationIdException ex)
            {
                throw new BO.BadStationIdException("This station already exists", ex);
            }
        }




        public void DeleteStation(int code)
        {
            try
            {

                dal.DeleteStationFromAllLines(code);
                dal.RemoveStation(code);

                foreach (Line line in GetAllLines())
                {
                    UpdateLineInfos(line);
                }

                
            }
            catch (DO.BadStationIdException ex)
            {
                throw new BO.BadStationIdException("This station doesn't exist", ex);
            }
        }






        public IEnumerable<Station> GetAllStations()
        {
            return from stationDO in dal.GetAllStation()
                   orderby stationDO.Code
                   select StationDoBoAdapter(stationDO);
        }





        public Station GetStation(int code)
        {
            DO.Station stationDO;
            try
            {
                stationDO = dal.GetStation(code);
            }
            catch (DO.BadStationIdException ex)
            {
                throw new BO.BadStationIdException("This station doesn't exist", ex);
            }
            return StationDoBoAdapter(stationDO);
        }

        public IEnumerable<Station> GetStationsBy(Predicate<Station> predicate)
        {
            return from station in dal.GetAllStation()
                   where predicate(StationDoBoAdapter(station))
                   select StationDoBoAdapter(station);
        }



        public void UpdateStationInfos(BO.Station station)
        {
            DO.Station stationDO = new DO.Station();
            station.CopyPropertiesTo(stationDO);
            try
            {
                dal.UpdateStation(stationDO);
            }
            catch (DO.BadStationIdException ex)
            {
                throw new BO.BadStationIdException("This station doesn't exist", ex);
            }
        }
        public IEnumerable<BO.LineStation> GetAllStationsPerLine(int id)
        {
            var ListOfStations = from ls in dal.GetLineStation(ls => ls.LineId == id)
                                 let station = dal.GetStation(ls.Station)
                                 select station.CopyToLineStation(ls);
            foreach (LineStation ls in ListOfStations)
            {
                if (ls.NextStation != 2)
                {
                    ls.TimetoNext = dal.GetAdjacentStations(ls.Code, ls.NextStation).Time;
                    ls.DistancetoNext = dal.GetAdjacentStations(ls.Code, ls.NextStation).Distance;
                }

            }
            return ListOfStations;
        }


        #endregion

        #region LineStation     

        public void AddLineStation(int linID, int statID,int prev, int next)
        {
            try
            {
                dal.AddLineStation(linID, statID,prev,next);
            }
            catch (DO.BadLineIdStationIDException ex)
            {
                throw new BO.BadLineIdStationIDException("Line ID and station ID not exist", ex);
            }
        }

        

        public void DeleteLineStation(int linID, int statID)
        {
            try
            {
                dal.RemoveLineStation(linID, statID);
                
            }
            catch (DO.BadLineIdStationIDException ex)
            {
                throw new BO.BadLineIdStationIDException("Student ID and Course ID is Not exist", ex);
            }
        }

        public void UpdateStationDistanceToNext(LineStation linsta, double distance)
        {
            try
            {
                dal.UpdateAdjacentStations(linsta.Code,linsta.NextStation,distance);
            }
            catch(DO.BadStationIdException ex)
            {
                throw new BO.BadLineIdStationIDException("Student ID and Course ID is Not exist", ex);

            }

        }

        #endregion

        #region User

        BO.User userDoBoAdapter(DO.User userDO)
        {
            BO.User userBO = new BO.User();
            string Pseudo = userDO.UserName;
            try
            {
                userDO = dal.GetUser(Pseudo);
            }
            catch (DO.BadUserIdException ex)
            {
                throw new BO.BadUserIdException("Bus ID doesn't exist", ex);
            }

            userDO.CopyPropertiesTo(userBO);

            return userBO;
        }

        // Je vais verifier si y a 8 chiffres en supposant que les nouveaux bus seront recents et non d'avant 2018
        // A noter qu'au moment de l'affichage dans le PL essayer de mettre au format xxx-xx-xxx
        public void AddUser(User user)
        {
            DO.User userDO = new DO.User();
            user.CopyPropertiesTo(userDO);
            try
            {
                dal.AddUser(userDO);
                
            }
            catch (DO.BadUserIdException ex)
            {
                throw new BO.BadUserIdException("This user already exists", ex);
            }

        } 

        public User GetUser(string pseudo)
        {
            DO.User userDO;
            try
            {
                userDO = dal.GetUser(pseudo);
            }
            catch (DO.BadUserIdException ex)
            {
                throw new BO.BadUserIdException("This user doesn't exist", ex);
            }
            return userDoBoAdapter(userDO);
        }

        public void DeleteUser(string pseudo)
        {
            try
            {
                dal.RemoveUser(pseudo);
            }
            catch (DO.BadUserIdException ex)
            {
                throw new BO.BadUserIdException("User UserName doesn't exist", ex);
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            return from userDO in dal.GetAllUser()
                   orderby userDO.UserName
                   select userDoBoAdapter(userDO);
        }

        public IEnumerable<User> GetUserBy(Predicate<User> predicate)// Ils ont pas ete memamesh mais a completer si on le veut 
        {
            return from user in dal.GetAllUser()
                   where predicate(userDoBoAdapter(user))
                   select userDoBoAdapter(user);
        }
 
        public void UpdateUserInfos(BO.User user)
        {
            DO.User userDO = new DO.User();
            user.CopyPropertiesTo(userDO);
            try
            {
                dal.UpdateUser(userDO);
            }
            catch (DO.BadUserIdException ex)
            {
                throw new BO.BadUserIdException("This user doesn't exist", ex);
            }
        }

        public bool LogInVerify(User user)
        {
            DO.User userDO = new DO.User();
            user.CopyPropertiesTo(userDO);

            try
            {
                dal.LogInVerify(userDO);
               
            }
            catch (DO.BadUserIdException ex)
            {
                
                throw new BO.BadUserIdException("Wrong User or Password", ex);

            }
            return true;

        }
        public bool isWorker(User user)
        {
            bool check;
            DO.User userDO = new DO.User();
            user.CopyPropertiesTo(userDO);
            check=dal.isWorker(userDO);
            return check;
        }

        #endregion
        //public IEnumerable<LineTime> ListArrivalOfLine(int lineId, TimeSpan hour, int stationKey)
        //{
        //    //Calcul of TravelTime between first station of line and our station
            



        //    List<LineTime> listTiming = new List<LineTime>(); //initialize list of all timing for the specified line
           

        //}

        //internal TimeSpan CalculateTimeOfArrival(TimeSpan startOfTRavel, TimeSpan durationOfTravel)
        //{
        //    return startOfTRavel + durationOfTravel;
        //}

        //internal TimeSpan DurationOfTravel(BO.Line line, int stationKey)
        //{
        //    int rankOfStation = dl.GetLineStation(line.Id, stationKey).RankInLine;
        //    IEnumerable<DO.LineStation> stations = (from lineStat in dl.GetAllLineStationsBy(l => l.LineId == line.Id).ToList()
        //                                            where lineStat.RankInLine <= rankOfStation
        //                                            select lineStat).ToList();

        //    TimeSpan travelDuration = new TimeSpan();
        //    for (int i = 0; i < stations.Count() - 1; i++)
        //    {
        //        DO.Station station1 = StationBoDoAdapter(GetStation(stations.ElementAt(i).LineId));
        //        DO.Station station2 = StationBoDoAdapter(GetStation(stations.ElementAt(i + 1).LineId));

        //        travelDuration += ;
        //    }

        //    return travelDuration;
        //}

        //public IEnumerable<IGrouping<TimeSpan, LineTime>> StationTiming(BO.Station station, TimeSpan hour)
        //{
        //    //if (station.LinesThatPass == null)
        //    //    throw new BO.InexistantLineTripException("לא נמצעו נסיעות בשעות אלו עבור הקו המבוקש");
        //    try
        //    {
        //        List<LineTime> timing = new List<LineTime>();
        //        foreach (int lineId in station.LinesThatPass)
        //        {
        //            foreach (var item in ListArrivalOfLine(lineId, hour, station.BusStationKey))
        //                timing.Add(item);
        //        }

        //        return from item in timing
        //               group item by item.ExpectedTimeTillArrive;
        //    }
        //    catch (DO. ex)
        //    {
        //        throw new BO.("לא נמצעו נסיעות בשעות אלו עבור הקו המבוקש", ex);
        //    }

        //}
    }
}
