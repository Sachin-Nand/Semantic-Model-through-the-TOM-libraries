using Microsoft.AnalysisServices.Tabular;
using Database = Microsoft.AnalysisServices.Tabular.Database;
using DataType = Microsoft.AnalysisServices.Tabular.DataType;
using Hierarchy = Microsoft.AnalysisServices.Tabular.Hierarchy;
using Level = Microsoft.AnalysisServices.Tabular.Level;
using Measure = Microsoft.AnalysisServices.Tabular.Measure;
using Partition = Microsoft.AnalysisServices.Tabular.Partition;
using Server = Microsoft.AnalysisServices.Tabular.Server;
using Table = Microsoft.AnalysisServices.Tabular.Table;
using Spectre.Console;

namespace Tom_For_Fabric
{
    internal class Program
    {

        static void Main(string[] args)
        {
            AnsiConsole.MarkupLine("");

            AnsiConsole.MarkupLine($"In the new browser window enter your credentials to authenticate the service");
            string workspaceConnection = "powerbi://api.powerbi.com/v1.0/myorg/My%20Test%20Workspace";
            string connectString = $"DataSource={workspaceConnection};";

            Server server = new Server();
            server.Connect(connectString);
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"Authentication successful");

            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"Started creating the [Red]AdventureWorks Semantic Model[/]");
            AnsiConsole.MarkupLine("");
            Thread.Sleep(2000);
            string status = CreateSemanticModel("AdventureWorks Semantic Model", server);
            if (status == "Success")
            {
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Successfully created and deployed the [Red]AdventureWorks Semantic Model[/]");
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Please re-enter the datasource credentials to fetch the data from the source and then refresh the model");
            }
            else
            {

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Deployment of [Red]AdventureWorks Semantic Model[/] failed with error [Red]{status}[/]");
            }

        }
        public static string CreateSemanticModel(string DatabaseName, Server server)
        {

            try
            {
                string newDatabaseName = server.Databases.GetNewName(DatabaseName);

                var database = new Database()
                {
                    Name = newDatabaseName,
                    ID = newDatabaseName,
                    CompatibilityLevel = 1520,
                    StorageEngineUsed = Microsoft.AnalysisServices.StorageEngineUsed.TabularMetadata,
                    Model = new Microsoft.AnalysisServices.Tabular.Model()
                    {
                        Name = DatabaseName + "AdventureWorks",
                        Description = "AdventureWorks start schema using Tabular Object Model(TOM)"
                    }
                };
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating the [Red]Customers[/] table in the model");
                Table tableCustomers = CreateCustomers(database, server);
                database.Model.Tables.Add(tableCustomers);
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating the [Red]Products[/] table in the model");
                Table tableProduct = CreateProducts(database, server);
                database.Model.Tables.Add(tableProduct);
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating the [Red]Product Category[/] table in the model");
                Table tableProductCategory = CreateProductCategory(database, server);
                database.Model.Tables.Add(tableProductCategory);
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating the [Red]Product Sub Category[/] table in the model");
                Table tableProductSubcategory = CreateProductSubcategory(database, server);
                database.Model.Tables.Add(tableProductSubcategory);
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating the [Red]Sales Territory[/] table in the model");
                Table tableSalesTerritory = CreateSalesTerritory(database, server);
                database.Model.Tables.Add(tableSalesTerritory);
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating the [Red]Calendar[/] table in the model");
                Table tableCalendarDB = CreateCalendarTable(database, server);
                database.Model.Tables.Add(tableCalendarDB);
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating the [Red]Fact Sales[/] table in the model");
                Table tableFactSales = CreateFactSales(database, server);
                database.Model.Tables.Add(tableFactSales);
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating Relationship between [Red]ProductCategory[/] and [Red]ProductSubcategory[/]");
                database.Model.Relationships.Add(new SingleColumnRelationship
                {
                    Name = "ProductCategory to ProductSubcategory",
                    ToColumn = tableProductCategory.Columns["ProductCategoryID"],
                    ToCardinality = RelationshipEndCardinality.One,
                    FromColumn = tableProductSubcategory.Columns["ProductCategoryID"],
                    FromCardinality = RelationshipEndCardinality.Many
                });
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating Relationship between [Red]Customers[/] and [Red]FactSales[/]");
                database.Model.Relationships.Add(new SingleColumnRelationship
                {
                    Name = "Customers to FactSales",
                    ToColumn = tableCustomers.Columns["CustomerID"],
                    ToCardinality = RelationshipEndCardinality.One,
                    FromColumn = tableFactSales.Columns["CustomerID"],
                    FromCardinality = RelationshipEndCardinality.Many
                });
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating Relationship between [Red]Products[/] and [Red]FactSales[/]");
                database.Model.Relationships.Add(new SingleColumnRelationship
                {
                    Name = "Products to FactSales",
                    ToColumn = tableProduct.Columns["ProductID"],
                    ToCardinality = RelationshipEndCardinality.One,
                    FromColumn = tableFactSales.Columns["ProductID"],
                    FromCardinality = RelationshipEndCardinality.Many
                });
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating Relationship between [Red]SalesTerritory[/] and [Red]FactSales[/]");
                database.Model.Relationships.Add(new SingleColumnRelationship
                {
                    Name = "SalesTerritory to FactSales",
                    ToColumn = tableSalesTerritory.Columns["TerritoryID"],
                    ToCardinality = RelationshipEndCardinality.One,
                    FromColumn = tableFactSales.Columns["TerritoryID"],
                    FromCardinality = RelationshipEndCardinality.Many
                });
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating Relationship between [Red]ProductSubCategory[/] and [Red]Category[/]");

                database.Model.Relationships.Add(new SingleColumnRelationship
                {
                    Name = "Product to ProductSubcategory",
                    ToColumn = tableProductSubcategory.Columns["ProductSubcategoryID"],
                    ToCardinality = RelationshipEndCardinality.One,
                    FromColumn = tableProduct.Columns["ProductSubcategoryID"],
                    FromCardinality = RelationshipEndCardinality.Many
                });
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating Relationship between [Red]Calendar[/] and [Red]FactSales[/]");
                database.Model.Relationships.Add(new SingleColumnRelationship
                {
                    Name = "Date to SalesOrderDate",
                    FromColumn = tableFactSales.Columns["OrderDate"],
                    FromCardinality = RelationshipEndCardinality.Many,
                    ToColumn = tableCalendarDB.Columns["Date"],
                    ToCardinality = RelationshipEndCardinality.One,
                    IsActive = true

                });
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating Relationship across [Red]ProductCategory[/] to [Red]ProductSubcategory[/]");
                database.Model.Relationships.Add(new SingleColumnRelationship
                {
                    Name = "Date to SalesDueDate",
                    FromColumn = tableFactSales.Columns["DueDate"],
                    FromCardinality = RelationshipEndCardinality.Many,
                    ToColumn = tableCalendarDB.Columns["Date"],
                    ToCardinality = RelationshipEndCardinality.One,
                    IsActive = false

                });
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Creating Relationship across [Red]ProductCategory[/] to [Red]ProductSubcategory[/]");
                database.Model.Relationships.Add(new SingleColumnRelationship
                {
                    Name = "Date to SalesShipDate",
                    FromColumn = tableFactSales.Columns["ShipDate"],
                    FromCardinality = RelationshipEndCardinality.Many,
                    ToColumn = tableCalendarDB.Columns["Date"],
                    ToCardinality = RelationshipEndCardinality.One,
                    IsActive = false
                });
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"Done...");
                Thread.Sleep(2000);

                server.Databases.Add(database);
                server.Execute(Microsoft.AnalysisServices.Tabular.JsonScripter.ScriptCreate(database));
                database.Model.RequestRefresh(Microsoft.AnalysisServices.Tabular.RefreshType.Full);

                return "Success";
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static Table CreateCustomers(Database database, Server server)
        {
            Table tableCustomers = new Table()
            {

                Name = database.Model.Tables.GetNewName("Dim_Customers"),
                Description = "Customer Details",
                Partitions = {

                        new Partition() {
                            Name = "All Customers",
                            Source = new MPartitionSource() {

                               Expression =  Properties.Resources.Customer
                            }
                        }
                    },
                Columns =
                    {
                      new DataColumn() {
                          Name = "CustomerID",
                            DataType = DataType.Int64,
                            SourceColumn = "CustomerID",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "AccountNumber",
                            DataType = DataType.String,
                            SourceColumn = "AccountNumber",
                            SummarizeBy = AggregateFunction.None
                        },

                        new DataColumn() {
                          Name = "FirstName",
                            DataType = DataType.String,
                            SourceColumn = "FirstName",
                            SummarizeBy = AggregateFunction.None
                        },
                        new DataColumn() {
                            Name = "LastName",
                            DataType = DataType.String,
                            SourceColumn = "LastName",
                            SummarizeBy = AggregateFunction.None
                        }
                    }
            };

            return tableCustomers;
        }


        public static Table CreateProducts(Database database, Server server)
        {
            Table tableProduct = new Table()
            {
                Name = database.Model.Tables.GetNewName("Dim_Products"),
                Description = "Product Details",
                Partitions = {

                        new Partition() {
                            Name = "All Products",
                            Source = new MPartitionSource() {

                               Expression =  Properties.Resources.Product
                            }
                        }
                    },
                Columns =
                    {
                      new DataColumn() {
                          Name = "ProductID",
                            DataType = DataType.Int64,
                            SourceColumn = "ProductID",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "ProductSubcategoryID",
                            DataType = DataType.Int64,
                            SourceColumn = "PSC_ID_1",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Product",
                            DataType = DataType.String,
                            SourceColumn = "ProductName",
                            SummarizeBy = AggregateFunction.None
                        },

                        new DataColumn() {
                          Name = "ProductNumber",
                            DataType = DataType.String,
                            SourceColumn = "ProductNumber",
                            SummarizeBy = AggregateFunction.None
                        },
                          new DataColumn() {
                          Name = "ProductCategoryID",
                            DataType = DataType.String,
                            SourceColumn = "PC_ID_1",
                            SummarizeBy = AggregateFunction.None
                        },
                           new DataColumn() {
                          Name = "ProductSubCategory",
                            DataType = DataType.String,
                            SourceColumn = "ProductSubCategoryName",
                            SummarizeBy = AggregateFunction.None
                        },
                            new DataColumn() {
                          Name = "ProductCategory",
                            DataType = DataType.String,
                            SourceColumn = "ProductCategoryName",
                            SummarizeBy = AggregateFunction.None
                        },
                    },

            };

            tableProduct.Hierarchies.Add(

                new Hierarchy()
                {
                    Name = "Product Hierarchy",
                    Levels = {
                      new Level() { Ordinal=0, Name="ProductCategory", Column=tableProduct.Columns["ProductCategory"]  },
                      new Level() { Ordinal=1, Name="ProductSubCategory", Column=tableProduct.Columns["ProductSubCategory"] },
                      new Level() { Ordinal=2, Name="Product", Column=tableProduct.Columns["Product"] }
                }


                }

                );
            return tableProduct;

        }

        public static Table CreateSalesTerritory(Database database, Server server)
        {
            Table tableSalesTerritory = new Table()
            {
                Name = database.Model.Tables.GetNewName("Dim_SalesTerritory"),
                Description = "SalesTerritory Details",
                Partitions = {

                        new Partition() {
                            Name = "All SalesTerritory",
                            Source = new MPartitionSource() {

                               Expression =  Properties.Resources.SalesTerritory
                            }
                        }
                    },
                Columns =
                    {
                      new DataColumn() {
                          Name = "TerritoryID",
                            DataType = DataType.Int64,
                            SourceColumn = "TerritoryID",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Name",
                            DataType = DataType.String,
                            SourceColumn = "Name",
                            SummarizeBy = AggregateFunction.None
                        }
                       ,
                       new DataColumn() {
                          Name = "ModifiedDate",
                            DataType = DataType.DateTime,
                            SourceColumn = "ModifiedDate",
                            SummarizeBy = AggregateFunction.None
                        }
                    }
            };

            return tableSalesTerritory;

        }

        public static Table CreateProductCategory(Database database, Server server)
        {
            Table tableProductCategory = new Table()


            {
                Name = database.Model.Tables.GetNewName("Dim_ProductCategory"),
                Description = "ProductCategory Details",
                Partitions = {

                        new Partition() {
                            Name = "All ProductCategory",
                            Source = new MPartitionSource() {

                               Expression =  Properties.Resources.ProductCategory
                            }
                        }
                    },
                Columns =
                    {
                      new DataColumn() {
                          Name = "ProductCategoryID",
                            DataType = DataType.Int64,
                            SourceColumn = "ProductCategoryID",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Name",
                            DataType = DataType.String,
                            SourceColumn = "Name",
                            SummarizeBy = AggregateFunction.None
                        },
                    }
            };
            return tableProductCategory;


        }
        public static Table CreateProductSubcategory(Database database, Server server)
        {


            Table tableProductSubcategory = new Table()
            {
                Name = database.Model.Tables.GetNewName("Dim_ProductSubCategory"),
                Description = "ProductSubcategory Details",
                Partitions = {

                        new Partition() {
                            Name = "All ProductSubcategory",
                            Source = new MPartitionSource() {

                               Expression =  Properties.Resources.ProductSubCategory
                            }
                        }
                    },
                Columns =
                    {
                      new DataColumn() {
                          Name = "ProductSubcategoryID",
                            DataType = DataType.Int64,
                            SourceColumn = "ProductSubcategoryID",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "ProductCategoryID",
                            DataType = DataType.Int64,
                            SourceColumn = "ProductCategoryID",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Name",
                            DataType = DataType.String,
                            SourceColumn = "Name",
                            SummarizeBy = AggregateFunction.None
                        },
                    }
            };
            return tableProductSubcategory;
        }


        public static Table CreateFactSales(Database database, Server server)
        {


            Table tableFactSales = new Table()
            {
                Name = database.Model.Tables.GetNewName("FactSales"),
                Description = "Sales Details",
                Partitions = {

                        new Partition() {
                            Name = "All SalesOrders",
                            Source = new MPartitionSource() {

                               Expression =  Properties.Resources.FactSales
                            }
                        }
                    },
                Columns =
                    {
                      new DataColumn() {
                          Name = "SalesOrderID",
                            DataType = DataType.Int64,
                            SourceColumn = "SalesOrderID_H",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "CustomerID",
                            DataType = DataType.Int64,
                            SourceColumn = "CustomerID",
                            SummarizeBy = AggregateFunction.None
                        },
                        new DataColumn() {
                          Name = "ProductID",
                            DataType = DataType.Int64,
                            SourceColumn = "ProductID",
                            SummarizeBy = AggregateFunction.None
                        },
                          new DataColumn() {
                          Name = "TerritoryID",
                            DataType = DataType.Int64,
                            SourceColumn = "TerritoryID",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "SalesOrderNumber",
                            DataType = DataType.String,
                            SourceColumn = "SalesOrderNumber",
                            SummarizeBy = AggregateFunction.None
                        },
                        new DataColumn() {
                          Name = "PurchaseOrderNumber",
                            DataType = DataType.String,
                            SourceColumn = "PurchaseOrderNumber",
                            SummarizeBy = AggregateFunction.None
                        },

                        new DataColumn() {
                          Name = "OrderDate",
                            DataType = DataType.DateTime,
                            SourceColumn = "OrderDate",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "DueDate",
                            DataType = DataType.DateTime,
                            SourceColumn = "DueDate",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "ShipDate",
                            DataType = DataType.DateTime,
                            SourceColumn = "ShipDate",
                            SummarizeBy = AggregateFunction.None
                        },
                          new DataColumn() {
                          Name = "CarrierTrackingNumber",
                            DataType = DataType.String,
                            SourceColumn = "CarrierTrackingNumber",
                            SummarizeBy = AggregateFunction.None
                        },
                            new DataColumn() {
                          Name = "OrderQty",
                            DataType = DataType.Int64,
                            SourceColumn = "OrderQty",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "UnitPrice",
                            DataType = DataType.Decimal,
                            SourceColumn = "UnitPrice",
                            SummarizeBy = AggregateFunction.None
                        },
                        new DataColumn() {
                          Name = "UnitPriceDiscount",
                            DataType = DataType.Decimal,
                            SourceColumn = "UnitPriceDiscount",
                            SummarizeBy = AggregateFunction.None
                        },


                    },


            };

            tableFactSales.Measures.Add(new Measure { Name = "CustomerCount", Expression = "Count(FactSales[CustomerID])" });
            tableFactSales.Measures.Add(new Measure { Name = "Total Quantity", Expression = "Sum(FactSales[OrderQty])", FormatString = @"\$#,0;(\$#,0);\$#,0" });
            return tableFactSales;
        }


        public static Table CreateCalendarTable(Database database, Server server)
        {
            Table tableCalendarDB = new Table()
            {
                Name = database.Model.Tables.GetNewName("Dim_Calendar"),
                Description = "SalesTerritory Details",
                Partitions = {

                        new Partition() {
                            Name = "All CalendarDate",
                            Source = new MPartitionSource() {

                               Expression =  Properties.Resources.Calendar
                            }
                        }
                    },
                Columns =
                    {
                      new DataColumn() {
                          Name = "Date",
                            DataType = DataType.DateTime,
                            SourceColumn = "Date",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Year",
                            DataType = DataType.Int64,
                            SourceColumn = "Year",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Month",
                            DataType = DataType.Int64,
                            SourceColumn = "Month_Number",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Month Name",
                            DataType = DataType.String,
                            SourceColumn = "Month_Name",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Day",
                            DataType = DataType.Int64,
                            SourceColumn = "Day",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Weekday",
                            DataType = DataType.Int64,
                            SourceColumn = "Weekday_Number",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Day Name",
                            DataType = DataType.String,
                            SourceColumn = "Weekday_Name",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "IsWeekend",
                            DataType = DataType.Boolean,
                            SourceColumn = "IsWeekend",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Quarter",
                            DataType = DataType.Int64,
                            SourceColumn = "Quarter",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Quarter Name",
                            DataType = DataType.String,
                            SourceColumn = "Quarter_Name",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Quarter Year",
                            DataType = DataType.String,
                            SourceColumn = "Year_Quarter",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Week Number",
                            DataType = DataType.Int64,
                            SourceColumn = "Week_Number",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Year Month",
                            DataType = DataType.String,
                            SourceColumn = "Year_Month",
                            SummarizeBy = AggregateFunction.None
                        },
                       new DataColumn() {
                          Name = "Is Leap Year",
                            DataType = DataType.Boolean,
                            SourceColumn = "IsLeapYear",
                            SummarizeBy = AggregateFunction.None
                        }

                    }
            };

            tableCalendarDB.Hierarchies.Add(new Hierarchy()
            {
                Name = "Calendar Hierarchy",
                Levels = {
                    new Level() { Ordinal = 0, Name = "Year", Column = tableCalendarDB.Columns["Year"] },
                  new Level() { Ordinal = 1, Name = "Quarter", Column = tableCalendarDB.Columns["Quarter Name"] },
                  new Level() { Ordinal = 2, Name = "Month", Column = tableCalendarDB.Columns["Month Name"] }
              }
            });

            return tableCalendarDB;

        }



    }




}




