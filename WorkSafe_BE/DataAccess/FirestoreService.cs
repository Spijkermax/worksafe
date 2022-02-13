﻿using Google.Cloud.Firestore;
using WorkSafe_BE.Models;

namespace WorkSafe_BE.DataAccess
{
    public class FirestoreService : IDatabaseService
    {
        public FirestoreService()
        {
            string credential_path = @"./DataAccess/worksafe-f99a3-dfd3441ac6fd.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);
            _db = FirestoreDb.Create(_projectId);
        }

        private FirestoreDb _db;
        private string _projectId = "worksafe-f99a3";


        /// <summary>
        /// Adds a User to Firestore
        /// </summary>
        /// <param name="project">A UsertModel containing the Project to add</param>
        /// <returns>The Id of the user as a string</returns>
        public async Task<string> AddUser(UserModel user)
        {           
            DocumentReference docRef = _db.Collection("Users").Document(user.Id);
            Dictionary<string, object> userDictionary = new Dictionary<string, object>
            {             
                { "Name", user.Name },
                { "NickName", user.NickName },
                { "Email", user.Email },
                { "Picture", user.Picture },
                { "TimeStamp", Timestamp.FromDateTime(user.TimeStamp) },
            };
            if (!await UserExists(user.Id))
            {
                await docRef.SetAsync(userDictionary);
            }            
            return docRef.Id;
        }

        /// <summary>
        /// Gets a User by Id
        /// </summary>
        /// <param name="id">The id of the user as a string</param>
        /// <returns>A UserModel containing the user or null of user with given id is not in the db</returns>
        public async Task<UserModel?> GetUser(string id)
        {           
            DocumentReference docRef = _db.Collection("Users").Document(id);
            DocumentSnapshot document = await docRef.GetSnapshotAsync();
            if (document.Exists == true)
            {               
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                var user = new UserModel(id, documentDictionary);
                return user;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Updates a user in firestore
        /// </summary>
        /// <param name="user">A usermodel containg the user to update</param>
        /// <returns>The id of the user that was updated</returns>
        public async Task<string?> UpdateUser(UserModel user)
        {
            //updating a user uses the same code as creating in firestore
            //possible we may want to validate the user exists
            return await AddUser(user);
        }

        /// <summary>
        /// Deletes a user from firestore
        /// </summary>
        /// <param name="id">The Id of the user to delete</param>
        /// <returns>The Id of the user that was deleted</returns>
        public async Task<string?> DeleteUser(string id)
        {
            DocumentReference docRef = _db.Collection("Users").Document(id);
            await docRef.DeleteAsync();
            return docRef.Id;
        }




        /// <summary>
        /// Gets a list of all users from firebase
        /// </summary>
        /// <returns>A list of UserModels</returns>
        public async Task<List<UserModel>> GetUsers()
        {
            var output = new List<UserModel>();
            CollectionReference usersRef = _db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                var user = new UserModel(document.Id, documentDictionary);
                output.Add(user);
            }
            return output;
        }

        /// <summary>
        /// Checks if a user already exists in the db
        /// </summary>
        /// <param name="userid">The user id to check is in db</param>
        /// <returns>True if present, false otherwise</returns>
        public async Task<bool>UserExists(string userid)
        {
            var doc = await _db.Collection("Users").Document(userid).GetSnapshotAsync();
            if (doc.Exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        /// <summary>
        /// Adds a Project to Firestore
        /// </summary>
        /// <param name="project">A ProjectModel containing the Project to add</param>
        /// <returns>The autogenerated Id of the project as a string</returns>
        public async Task<string> AddProject(ProjectModel project)
        {
            DocumentReference docRef = _db.Collection("Projects").Document();
            Dictionary<string, object> projectDictionary = new Dictionary<string, object>
            {
                { "Title", project.Title },
                { "Description", project.Description },
                { "ProjectGoal", project.ProjectGoal },
                { "PillarConnection", project.PillarConnection },
                { "PillarEmbedding", project.PillarEmbedding },
                { "PillarLeadership", project.PillarLeadership },
                { "PillarNeeds", project.PillarNeeds },
                { "PillarResources", project.PillarResources },
                { "TimeStamp", Timestamp.FromDateTime(project.TimeStamp) },
                { "CreationTime", Timestamp.FromDateTime(project.CreationTime) },
                { "OwnerId", project.Owner.Id },
                { "LastUpdatedById", project.LastUpdatedBy.Id },
                { "Color", project.Color },

            };
            await docRef.SetAsync(projectDictionary);
            //need to also add collection of Collaborators possibly later
            return docRef.Id;
        }

        /// <summary>
        /// Gets a Project by Id
        /// </summary>
        /// <param name="id">The id of the project as a string</param>
        /// <returns>A ProjectModel containing the project</returns>
        public async Task<ProjectModel?> GetProject(string id)
        {
            DocumentReference docRef = _db.Collection("Projects").Document(id);
            DocumentSnapshot document = await docRef.GetSnapshotAsync();
            if (document.Exists == true)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                Task<UserModel?> ownerTask = GetUser((string)documentDictionary["OwnerId"]);
                Task<UserModel?> lastUpdatedByTask = GetUser((string)documentDictionary["LastUpdatedById"]);
                await Task.WhenAll(ownerTask, lastUpdatedByTask);
                var owner = await ownerTask;
                var lastUpdatedBy = await lastUpdatedByTask;


                //replace empty list of collaborators with actual list at some point
                var collaborators = new List<UserModel>();
                var project = new ProjectModel(document.Id, documentDictionary, owner, lastUpdatedBy);
                return project;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Gets a list of all projects from firebase
        /// </summary>
        /// <returns>A list of ProjectModels</returns>
        public async Task<List<ProjectModel>> GetProjects()
        {
            var output = new List<ProjectModel>();
            CollectionReference projectsRef = _db.Collection("Projects");
            Query query = projectsRef.OrderByDescending("CreationTime");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                Task<UserModel?> ownerTask = GetUser((string)documentDictionary["OwnerId"]);
                Task<UserModel?> lastUpdatedByTask = GetUser((string)documentDictionary["LastUpdatedById"]);
                await Task.WhenAll(ownerTask, lastUpdatedByTask);
                var owner = await ownerTask;
                var lastUpdatedBy = await lastUpdatedByTask;
                var project = new ProjectModel(document.Id, documentDictionary, owner, lastUpdatedBy);
                output.Add(project);
            }

            return output;
        }

        /// <summary>
        /// Updates a user in firestore
        /// </summary>
        /// <param name="user">A usermodel containg the user to update</param>
        /// <returns>The id of the user that was updated</returns>
        public async Task<string?> UpdateProject(ProjectModel project)
        {
            DocumentReference docRef = _db.Collection("Projects").Document(project.Id);
            Dictionary<string, object> projectDictionary = new Dictionary<string, object>
            {
                { "Title", project.Title },
                { "Description", project.Description },
                { "ProjectGoal", project.ProjectGoal },
                { "PillarConnection", project.PillarConnection },
                { "PillarEmbedding", project.PillarEmbedding },
                { "PillarLeadership", project.PillarLeadership },
                { "PillarNeeds", project.PillarNeeds },
                { "PillarResources", project.PillarResources },
                { "TimeStamp", Timestamp.FromDateTime(project.TimeStamp) },
                { "CreationTime", Timestamp.FromDateTime(project.CreationTime) },
                { "OwnerId", project.Owner.Id },
                { "LastUpdatedById", project.LastUpdatedBy.Id },
                { "Color", project.Color },
            };
            await docRef.SetAsync(projectDictionary);
            //need to also add collection of Collaborators possibly later
            return docRef.Id;
        }

        /// <summary>
        /// Deletes a user from firestore
        /// </summary>
        /// <param name="id">The Id of the user to delete</param>
        /// <returns>The Id of the user that was deleted</returns>
        public async Task<string?> DeleteProject(string id)
        {
            DocumentReference docRef = _db.Collection("Projects").Document(id);
            await docRef.DeleteAsync();
            return docRef.Id;
        }


        /// <summary>
        /// Checks if a project already exists in the db
        /// </summary>
        /// <param name="projectid">The project id to check is in db</param>
        /// <returns>True if present, false otherwise</returns>
        public async Task<bool> ProjectExists(string projectid)
        {
            var doc = await _db.Collection("Projects").Document(projectid).GetSnapshotAsync();
            if (doc.Exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Adds an Entry to Firestore
        /// </summary>
        /// <param name="project">A ProjectModel containing the Project to add</param>
        /// <returns>The autogenerated Id of the project as a string</returns>
        public async Task<string> AddEntry(EntryModel entry)
        {
            if (!await UserExists(entry.Author.Id))
            {
                return "Error";
            }

            DocumentReference userDocRef = _db.Collection("Users").Document(entry.Author.Id).Collection("Entries").Document();
            Dictionary<string, object> entryDictionary = new Dictionary<string, object>
            {
                { "Title", entry.Title },
                { "Description", entry.Description },
                { "TimeStamp", Timestamp.FromDateTime(entry.TimeStamp) },
                { "EntryDate", Timestamp.FromDateTime(entry.EntryDate) },
                { "AuthorId", entry.Author.Id },
                { "Files", entry.Files },
                { "Impact", entry.Impact },
                { "Learning", entry.Learning},
                { "MindSet", entry.MindSet },
                { "NextSteps", entry.NextSteps },
                { "Tags", entry.Tags },
                {"ProjectId", "" },
            };


            if (!entry.Project.Id.Equals(""))
            {
                entryDictionary["ProjectId"] = entry.Project.Id;
                DocumentReference projDocRef = _db.Collection("Projects").Document(entry.Project.Id).Collection("Entries").Document(userDocRef.Id);
                //add to the project if a project id has been set that matches a project in the db
                if (await ProjectExists(entry.Project.Id))
                {
                    await projDocRef.SetAsync(entryDictionary);

                    //update the project timestamp
                    var project = await GetProject(entry.Project.Id);
                    if (project != null)
                    {
                        project.TimeStamp = DateTime.UtcNow;
                        project.LastUpdatedBy.Id = entry.Author.Id;
                        await UpdateProject(project);
                    }
                }

            }

            if (entry.Tags.Any()) {
                await AddTags(entry.Tags);
            }
            
            await userDocRef.SetAsync(entryDictionary);
            return userDocRef.Id;
        }

        /// <summary>
        /// Gets a Project by Id
        /// </summary>
        /// <param name="id">The id of the project as a string</param>
        /// <returns>A ProjectModel containing the project</returns>
        public async Task<EntryModel> GetEntry(string id, string parentId, TopCollection topCollection)
        {
            DocumentReference docRef = _db.Collection(topCollection.ToString()).Document(parentId).Collection("Entries").Document(id);
            DocumentSnapshot document = await docRef.GetSnapshotAsync();
            Dictionary<string, object> documentDictionary = document.ToDictionary();
            var author = await GetUser((string)documentDictionary["AuthorId"]);
            var projectId = (string)documentDictionary["ProjectId"];
            EntryModel entry;
            if (!projectId.Equals(""))
            {
                var project = await GetProject(projectId);
                entry = new EntryModel(document.Id, documentDictionary, author, project);
            }
            else
            {
                entry = new EntryModel(document.Id, documentDictionary, author);
            }
            return entry;
        }

        public async Task<List<EntryModel>> GetEntries(string id, TopCollection topCollection, DateTime? startDate = null, DateTime? endDate = null, int? startEntryNum = null, int? numEntries = null, string[]? tags = null, string? orderBy = null)
        {
            var output = new List<EntryModel>();
            CollectionReference entriesRef = _db.Collection(topCollection.ToString()).Document(id).Collection("Entries");

            Query query = entriesRef;

            if (startDate != null)
            {
                query = query.WhereGreaterThanOrEqualTo ("EntryDate", startDate);
            }

            if (endDate != null)
            {
                query = query.WhereLessThanOrEqualTo("EntryDate", endDate);
            }

            if (startEntryNum != null)
            {
                startEntryNum = 0;
            }

            if (numEntries != null)
            {
                query = query.StartAt(startEntryNum).Limit((int)numEntries);
            }

            if (tags != null && tags.Length > 0)
            {
                query = query.WhereArrayContainsAny("Tags", tags);
            }

            if (orderBy != null)
            {
                query = query.OrderByDescending(orderBy);
            }
            else
            {
                //default to sorting by entry date
                query = query.OrderByDescending("EntryDate");
            }
            
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                var author = await GetUser((string)documentDictionary["AuthorId"]);
                var projectId = (string)documentDictionary["ProjectId"];
                EntryModel entry;
                if (!projectId.Equals(""))
                {
                    var project = await GetProject(projectId);
                    entry = new EntryModel(document.Id, documentDictionary, author, project);
                }
                else
                {
                    entry = new EntryModel(document.Id, documentDictionary, author);
                }
                output.Add(entry);
            }

            return output;
        }

        /// <summary>
        /// Updates an Entry in Firestore
        /// </summary>
        /// <param name="project">A ProjectModel containing the Project to add</param>
        /// <returns>The autogenerated Id of the project as a string</returns>
        public async Task<string> UpdateEntry(EntryModel entry)
        {
            if (!await UserExists(entry.Author.Id))
            {
                return "Error";
            }
            DocumentReference userDocRef = _db.Collection("Users").Document(entry.Author.Id).Collection("Entries").Document(entry.Id);

           
            Dictionary<string, object> entryDictionary = new Dictionary<string, object>
            {
                { "Title", entry.Title },
                { "Description", entry.Description },
                { "TimeStamp", Timestamp.FromDateTime(entry.TimeStamp) },
                { "EntryDate", Timestamp.FromDateTime(entry.EntryDate) },
                { "AuthorId", entry.Author.Id },
                { "Files", entry.Files },
                { "Impact", entry.Impact },
                { "Learning", entry.Learning},
                { "MindSet", entry.MindSet },
                { "NextSteps", entry.NextSteps },
                { "Tags", entry.Tags },
                { "ProjectId", "" },
            };

            if (!entry.Project.Id.Equals(""))
            {
                entryDictionary["ProjectId"] = entry.Project.Id;
                DocumentReference projDocRef = _db.Collection("Projects").Document(entry.Project.Id).Collection("Entries").Document(entry.Id);
                //add to the project if a project id has been set that matches a project in the db

                //if the project id in the passed in entry is not the same as the one in the database, we delete the entry from the projectId stored in the db - avoiding orphaned entries
                var dbEntry = await GetEntry(entry.Id, entry.Author.Id, TopCollection.Users);
                if (!dbEntry.Project.Id.Equals(entry.Project.Id))
                {
                    var projectEntryRef = _db.Collection("Projects").Document(dbEntry.Project.Id).Collection("Entries").Document(dbEntry.Id);
                    await projectEntryRef.DeleteAsync();
                }

                if (await ProjectExists(entry.Project.Id))
                {
                    await projDocRef.SetAsync(entryDictionary);
                    //need to update the project timestamp here
                    var project = await GetProject(entry.Project.Id);
                    if (project != null)
                    {
                        project.TimeStamp = DateTime.UtcNow;
                        project.LastUpdatedBy.Id = entry.Author.Id;
                        await UpdateProject(project);
                    }                    
                }
            }

            if (entry.Tags.Any()) {
                await AddTags(entry.Tags);
            }

            await userDocRef.SetAsync(entryDictionary);
            return userDocRef.Id;
        }

        /// <summary>
        /// Deletes an Entry from Firestore
        /// </summary>
        /// <param name="userid">The id of the author of the entry.</param>
        /// <param name="entryid">The id of the entry to delete</param>
        /// <returns>The id of the entry that was deleted</returns>
        public async Task<string> DeleteEntry(string userid, string entryid)
        {
            if (await EntryExists(userid, entryid))
            {
                var entry = await GetEntry(entryid, userid, TopCollection.Users);
                if (entry.Project != null)
                {
                    var projectEntryRef = _db.Collection("Projects").Document(entry.Project.Id).Collection("Entries").Document(entryid);
                    await projectEntryRef.DeleteAsync();
                }

                var userEntryRef = _db.Collection("Users").Document(userid).Collection("Entries").Document(entryid);
                await userEntryRef.DeleteAsync();

                return entryid;
            }
            else
            {
                return "Error";
            }
        }

        /// <summary>
        /// Checks if an entry already exists in the db
        /// </summary>
        /// <param name="projectid">The entry id to check is in db</param>
        /// <returns>True if present, false otherwise</returns>
        public async Task<bool> EntryExists(string userid, string entryid)
        {
            var doc = await _db.Collection("Users").Document(userid).Collection("Entries").Document(entryid).GetSnapshotAsync();
            if (doc.Exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //Tags

        /// <summary>
        /// Gets a list of all the tags currently in the database
        /// </summary>
        /// <returns>A List of the tags in the database</returns>
        public async Task<List<string>> GetTags()
        {
            var output = new List<string>();
            CollectionReference usersRef = _db.Collection("Tags");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                output.Add(document.Id);
            }
            return output;
        }

        /// <summary>
        /// Adds a list of tags to the database
        /// </summary>
        /// <param name="tags">The list of tags to add</param>
        /// <returns>???</returns>
        public async Task<List<string>> AddTags(List<string> tags)
        {
            var output = new List<string>();
            foreach(var tag in tags)
            {
                DocumentReference docRef = _db.Collection("Tags").Document(tag);
                Dictionary<string, object> tagDictionary = new Dictionary<string, object>
                {
                };

                await docRef.SetAsync(tagDictionary);

                //some kind of check here to make sure that the tags was added?
                output.Add(tag);
            }
            return output;
        }

        /// <summary>
        /// Updates a tag in the database
        /// </summary>
        /// <param name="oldTag"></param>
        /// <param name="newTag"></param>
        /// <returns></returns>
        public async Task<string> UpdateTag(string oldTag, string newTag)
        {
            await DeleteTag(oldTag);

            List<string> tags = new List<string>();
            tags.Add(newTag);

            var result = await AddTags(tags);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Deletes a tag from the database.
        /// </summary>
        /// <param name="tag">the tag to delete</param>
        /// <returns>The tag that was deleted</returns>
        public async Task<string> DeleteTag(string tag)
        {
            DocumentReference docRef = _db.Collection("Tags").Document(tag);
            await docRef.DeleteAsync();
            return docRef.Id;
        }


        //Colors

        /// <summary>
        /// Gets a list of all the colors currently in the database
        /// </summary>
        /// <returns>A List of the colors in the database</returns>
        public async Task<List<string>> GetColors()
        {
            var output = new List<string>();
            CollectionReference usersRef = _db.Collection("Colors");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                output.Add(document.Id);
            }
            return output;
        }

    }
}
