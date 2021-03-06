﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Persistences
{
    public class CrewMemberPersistence:BasePersistence
    {
       

        public CrewMemberPersistence(Models.Context CurrentContext)
        {
            this.CurrentContext = CurrentContext;
        }

        /// <summary>
        /// this method finds a crew member with the given pass confirmation id
        /// </summary>
        /// <param name="ConfirmationId"></param>
        /// <returns>Crew member that uses the given confirmation id</returns>
        public Models.CrewMember FindCrewMemberByConfirmationId(string ConfirmationId) {
            try
            {
                IQueryable<Models.CrewMember> _selectedCrewMember =
                                          from _crewMember in CurrentContext.CrewMembers
                                          where _crewMember.ConfirmAccountId == ConfirmationId
                                          select _crewMember;

                return _selectedCrewMember.First();
            }
            catch (System.InvalidOperationException E)
            {

                throw new Exceptions.CrewMemberNotFoundException();

            }
        }

        /// <summary>
        /// This method finds the crew member with the given reset password id
        /// </summary>
        /// <param name="ResetPassworId"></param>
        /// <returns></returns>
        public Models.CrewMember FindCrewMemberByResetPassworId(string ResetPassworId)
        {
            try
            {
                IQueryable<Models.CrewMember> _selectedCrewMember =
                                          from _crewMember in CurrentContext.CrewMembers
                                          where _crewMember.ResetPasswordId == ResetPassworId
                                          select _crewMember;

                return _selectedCrewMember.First();
            }
            catch (System.InvalidOperationException E)
            {

                throw new Exceptions.CrewMemberNotFoundException();

            }
        }


        /// <summary>
        /// this method finds the crew member with the given pass email
        /// </summary>
        /// <param name="CrewMemberEmail"></param>
        /// <returns>Crew member that uses the given email</returns>
        public Models.CrewMember FindCrewMemberByEmail(string CrewMemberEmail)
        {
            try
            {
                IQueryable<Models.CrewMember> _selectedCrewMember =
                                          from _crewMember in CurrentContext.CrewMembers
                                          where _crewMember.Email == CrewMemberEmail
                                          select _crewMember;

                return _selectedCrewMember.First();
            }
            catch (System.InvalidOperationException E) {

                return null;

            }
        }


        /// <summary>
        /// This method saves or updates the given crew member
        /// </summary>
        /// <param name="CrewMember"></param>
        /// <returns>the crew member after being saved or updated</returns>
        public Models.CrewMember AddOrUpdateCrewMember(Models.CrewMember CrewMember) {
            if (CrewMember.Id == 0)
            {
                
                    CurrentContext.CrewMembers.Add(CrewMember);
                    CurrentContext.SaveChanges();
                    CurrentContext.Entry(CrewMember).GetDatabaseValues();
           
            }
            else {
                    CurrentContext.SaveChanges();
            }
            return CrewMember;
        
        }

        /// <summary>
        /// This method finds all crew members
        /// </summary>
        /// <returns>All crew members</returns>
        public IQueryable<Models.CrewMember> FindAll() {
            try
            {
                return CurrentContext.CrewMembers;
            }
            catch (Exception E) {
                return null;
            }
        }


        /// <summary>
        /// This method finds a crew member that has the given id
        /// </summary>
        /// <param name="CrewMemberId"></param>
        /// <returns>The crew member with the given id</returns>
        public Models.CrewMember FindById(long CrewMemberId) {
            try
            {
                return CurrentContext.CrewMembers.Find(CrewMemberId);
            }
            catch (Exception E) {
                throw new Exceptions.CrewMemberNotFoundException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CrewMembers"></param>
        public void DeleteCrewMember(IQueryable<Models.CrewMember> CrewMembers) {
            foreach (Models.CrewMember _deletedCrewMember in CrewMembers) {
                CurrentContext.CrewMembers.Remove(_deletedCrewMember);
            }
            CurrentContext.SaveChanges();
        }

    }
}