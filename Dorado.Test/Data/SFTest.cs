using System;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Dorado.Data;
using Dorado.Data.Exceptions;
using Dorado.Data.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dorado.Test.Data
{
    /// <summary>
    ///This is a test class for db and is intended
    ///to contain all db Unit Tests
    ///</summary>
    [TestClass()]
    public class SafeProcedureTest
    {
        #region Additional test attributes

        //You can use the following additional attributes as you write your tests:
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }

        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        #endregion Additional test attributes

        private Database db = Database.GetDatabase("Tests");

        [Owner("Paul Walker")]
        [TestMethod]
        public void RunTests()
        {
            string name = "ImTheJuggernaurBitch";
            int rowsAffected = ExecuteNonQueryTest("dbo.AddUser", name);
            Assert.IsTrue(rowsAffected == 1, "db.ExecuteNonQuery failed to insert a user named " + name);
            int id = ExecuteScalarTest("dbo.GetUserByName", name);
            Assert.IsTrue(rowsAffected == 1, "db.ExecuteScalar failed to get the id of " + name);
            rowsAffected = ExecuteNonQueryTest("dbo.DeleteUserById", id);
            Assert.IsTrue(rowsAffected == 1, "db.ExecuteNonQuery failed to delete user with id " + id.ToString());
        }

        /// <summary>
        ///A test for ExecuteNonQuery (Database, string, params object[])
        ///</summary>
        private int ExecuteNonQueryTest(string procedureName, object paramValue)
        {
            object[] parameterValues = { paramValue };
            return db.ExecuteNonQuery(procedureName, parameterValues);
        }

        [TestMethod]
        public void ExecuteNonQueryExceptionsTest()
        {
            Database testDB = Database.GetDatabase("Tests");
            string procName = "dbo.Car_Update";

            try
            {
                testDB.ExecuteNonQuery(procName,
                 parameters =>
                 {
                     parameters.AddWithValue("carid", 582843357);
                     parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                 },
                 null);

                testDB.ExecuteNonQuery(procName,
                    parameters =>
                    {
                        parameters.AddWithValue("carid", 582843357);
                        parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                    },
                    null);
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("database", x.ParamName);
            }

            try
            {
                testDB.ExecuteNonQuery(null,
                    parameters =>
                    {
                        parameters.AddWithValue("carid", 582843357);
                        parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                    },
                    null);
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("procedureName", x.ParamName);
            }

            try
            {
                // Non-existent sproc
                testDB.ExecuteNonQuery("86eaf8e9310341e1aba7cc5475934596",
                     parameters =>
                     {
                         parameters.AddWithValue("carid", 582843357);
                         parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                     },
                    null);
                Assert.Fail();
            }
            catch (SafeProcedureException x)
            {
                Assert.AreEqual(testDB, x.Database);
                Assert.IsNotInstanceOfType(x.InnerException, typeof(SafeProcedureException));
            }

            Database fakeDB = Database.GetDatabase("DBWithInvalidConnectionString");
            try
            {
                // Non-existent connection
                fakeDB.ExecuteNonQuery(procName,
                     parameters =>
                     {
                         parameters.AddWithValue("carid", 582843357);
                         parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                     },
                    null);
                Assert.Fail();
            }
            catch (SafeProcedureException x)
            {
                Assert.AreEqual(fakeDB, x.Database);
            }
        }

        [TestMethod]
        public void ExecuteNonQueryWithConnectionExceptionsTest()
        {
            Database testDB = Database.GetDatabase("Tests");
            string procName = "dbo.Car_Update";

            try
            {
                using (SqlConnection connection = testDB.GetConnection())
                {
                    testDB.ExecuteNonQuery(connection, procName,
                         parameters =>
                         {
                             parameters.AddWithValue("carid", 582843357);
                             parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                         },
                        null);
                }
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("database", x.ParamName);
            }

            try
            {
                using (SqlConnection connection = testDB.GetConnection())
                {
                    testDB.ExecuteNonQuery(null, procName,
                         parameters =>
                         {
                             parameters.AddWithValue("carid", 582843357);
                             parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                         },
                        null);
                }
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("connection", x.ParamName);
            }

            try
            {
                using (SqlConnection connection = testDB.GetConnection())
                {
                    testDB.ExecuteNonQuery(connection, null,
                         parameters =>
                         {
                             parameters.AddWithValue("carid", 582843357);
                             parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                         },
                        null);
                }
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("procedureName", x.ParamName);
            }

            try
            {
                using (SqlConnection connection = testDB.GetConnection())
                {
                    testDB.ExecuteNonQuery(connection, "86eaf8e9310341e1aba7cc5475934596",
                         parameters =>
                         {
                             parameters.AddWithValue("carid", 582843357);
                             parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                         },
                        null);
                }
                Assert.Fail();
            }
            catch (SafeProcedureException x)
            {
                Assert.AreEqual(testDB, x.Database);
            }
        }

        /// <summary>
        ///A test for ExecuteScalar (Database, string, params object[])
        ///</summary>
        public int ExecuteScalarTest(string procedureName, object paramValue)
        {
            object retVal = db.ExecuteScalar(procedureName, paramValue);
            int id = Convert.ToInt32(retVal);
            return id;
        }

        [TestMethod]
        public void ExecuteScalarWithConnectionExceptionsTest()
        {
            Database testDB = Database.GetDatabase("Tests");
            string procName = "dbo.Car_Update";

            try
            {
                using (SqlConnection connection = testDB.GetConnection())
                {
                    testDB.ExecuteScalar(connection, procName,
                         parameters =>
                         {
                             parameters.AddWithValue("carid", 582843357);
                             parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                         },
                        null);
                }
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("database", x.ParamName);
            }

            try
            {
                using (SqlConnection connection = testDB.GetConnection())
                {
                    testDB.ExecuteScalar(null, procName,
                         parameters =>
                         {
                             parameters.AddWithValue("carid", 582843357);
                             parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                         },
                        null);
                }
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("connection", x.ParamName);
            }

            try
            {
                using (SqlConnection connection = testDB.GetConnection())
                {
                    testDB.ExecuteScalar(connection, null,
                        parameters =>
                        {
                            parameters.AddWithValue("carid", 582843357);
                            parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                        },
                        null);
                }
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("procedureName", x.ParamName);
            }

            try
            {
                using (SqlConnection connection = testDB.GetConnection())
                {
                    testDB.ExecuteScalar(connection, "86eaf8e9310341e1aba7cc5475934596",
                         parameters =>
                         {
                             parameters.AddWithValue("carid", 582843357);
                             parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                         },
                        null);
                }
                Assert.Fail();
            }
            catch (SafeProcedureException x)
            {
                Assert.AreEqual(testDB, x.Database);
            }
        }

        [TestMethod]
        public void ExecuteScalarExceptionsTest()
        {
            Database testDB = Database.GetDatabase("Tests");
            string procName = "dbo.Car_Update";

            try
            {
                testDB.ExecuteScalar(null, procName,
                     parameters =>
                     {
                         parameters.AddWithValue("carid", 582843357);
                         parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                     },
                    null);
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("database", x.ParamName);
            }

            try
            {
                testDB.ExecuteScalar(null,
                     parameters =>
                     {
                         parameters.AddWithValue("carid", 582843357);
                         parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                     },
                    null);
                Assert.Fail();
            }
            catch (ArgumentNullException x)
            {
                Assert.AreEqual("procedureName", x.ParamName);
            }

            try
            {
                // Non-existent sproc
                testDB.ExecuteScalar("86eaf8e9310341e1aba7cc5475934596",
                    parameters =>
                    {
                        parameters.AddWithValue("carid", 582843357);
                        parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                    },
                    null);
                Assert.Fail();
            }
            catch (SafeProcedureException x)
            {
                Assert.AreEqual(testDB, x.Database);
                Assert.IsNotInstanceOfType(x.InnerException, typeof(SafeProcedureException));
            }

            Database fakeDB = Database.GetDatabase("DBWithInvalidConnectionString");
            try
            {
                // Non-existent connection
                testDB.ExecuteScalar(null, procName,
                     parameters =>
                     {
                         parameters.AddWithValue("carid", 582843357);
                         parameters.AddWithValue("title", "20119166731541fdade75a790ae3eca5");
                     },
                    null);
                Assert.Fail();
            }
            catch (SafeProcedureException x)
            {
                Assert.AreEqual(fakeDB, x.Database);
            }
        }

        [TestMethod]
        [Owner("Oleg Pylnev")]
        [ExpectedException(typeof(NoDboException))]
        public void MissingDboExceptionTest()
        {
            db.Execute("GetUserLogin",
                 parameters =>
                 {
                     parameters.AddWithValue("@UserId", "-1");
                 }
            );
        }

        [TestMethod]
        [Owner("Oleg Pylnev")]
        [ExpectedException(typeof(DatabaseExecutionException))]
        public void InvalidParametersExceptionTest()
        {
            db.Execute("dbo.GetUserLogin",
                parameters =>
                {
                    parameters.AddWithValue("@UserId", "-1");
                }
            );
        }

        [TestMethod]
        [Owner("Oleg Pylnev")]
        [ExpectedException(typeof(SafeProcedureException))]
        public void BadCodeInDelegateExceptionTest()
        {
            db.Execute("dbo.GetUserLogin",
                parameters =>
                {
                    throw new Exception("Error occured in parameterMapper delegate");
                }
            );
        }

        [TestMethod]
        [Owner("Oleg Pylnev")]
        public void SafeProcedureSerializationTest()
        {
            SafeProcedureException e = new SafeProcedureException(db, "some_sproc_call", typeof(string), new Exception("Test Exception"));

            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, e);
                s.Position = 0; // Reset stream position
                e = (SafeProcedureException)formatter.Deserialize(s);
            }

            // Test individual properties
            Assert.AreEqual(e.Database.InstanceName, db.InstanceName);
            Assert.AreEqual(e.InstanceType, typeof(string));
            Assert.AreEqual(e.ProcedureName, "some_sproc_call");
            Assert.AreEqual(e.InnerException.Message, "Test Exception");
        }

        [TestMethod()]
        [Owner("Owyn Richen")]
        public void SafeProcedureReturnValueTest()
        {
            int result = 0;

            // url for devsrv
            // string url = "http://c.Dorado.com/Groups/00017/55/55/17765555_l.jpg";
            // url for msdb3
            string url = "http://666chicks.com/images/banner.gif";
            Database.GetDatabase("Media").ExecuteNonQuery(

                "dbo.checkURLStatus",
                 paramset =>
                 {
                     paramset.AddWithValue("@RETURN_VALUE", result, ParameterDirectionWrap.ReturnValue);
                     paramset.AddWithValue("@Url", url);
                 },
                delegate(IParameterSet paramset)
                {
                    result = (int)paramset.GetValue("@RETURN_VALUE");
                }
                );

            Assert.AreEqual<int>(2, result);
        }

        [TestMethod()]
        [Owner("paul walker")]
        public void NullParameterValuesTest()
        {
            object[] parameterValues = { null, null, null, null, null, null, null };
            object retVal = db.ExecuteScalar("[dbo].[AddCar]", parameterValues);
            Assert.IsNotNull(retVal, "Failed to call proc with null parameterValues");
            int id = Convert.ToInt32(retVal);
            db.ExecuteNonQuery("[dbo].[DeleteCarById]", id);
        }

        [TestMethod()]
        [Owner("paul walker")]
        public void NullParameterValuesWithDelegateTest()
        {
            object retVal = db.ExecuteScalar("[dbo].[AddCar]",
                delegate(IParameterSet parameters)
                {
                    parameters.AddWithValue("@Title", null);
                    parameters.AddWithValue("@Model", null);
                    parameters.AddWithValue("@Price", null);
                    parameters.AddWithValue("@Weight", null);
                    parameters.AddWithValue("@Manufactured", null);
                    parameters.AddWithValue("@Sold", null);
                    parameters.AddWithValue("@RTI", null);
                });

            Assert.IsNotNull(retVal, "Failed to call proc with null parameterValues");
            int id = Convert.ToInt32(retVal);
            db.ExecuteNonQuery("[dbo].[DeleteCarById]", id);
        }
    }
}