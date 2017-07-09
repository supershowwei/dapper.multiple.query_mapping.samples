using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dapper;
using DapperMultipleQueryAndMappingSamples.Model.Data;
using Newtonsoft.Json;

namespace DapperMultipleQueryAndMappingSamples
{
    public partial class Form1 : Form
    {
        private static readonly string ConnectionString =
            File.ReadAllText(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "connectionstring"));

        public Form1()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var order = default(Order);
            using (var db = new SqlConnection(ConnectionString))
            {
                var sql = @"
SELECT
	*
FROM [Order] o WITH (NOLOCK)
JOIN Customer c WITH (NOLOCK)
	ON o.CustomerId = c.Id
WHERE o.Id = @OrderId";

                order = db.Query<Order, Customer, Order>(
                    sql,
                    (o, c) =>
                        {
                            o.Customer = c;

                            return o;
                        },
                    new { OrderId = 1 },
                    splitOn: "Id").Single();
            }

            this.textBox1.Text = JsonConvert.SerializeObject(order, Formatting.Indented);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var order = default(Order);
            using (var db = new SqlConnection(ConnectionString))
            {
                var sql = @"
SELECT
	*
FROM [Order] o WITH (NOLOCK)
JOIN Customer c WITH (NOLOCK)
	ON o.CustomerId = c.Id
WHERE o.Id = @OrderId

SELECT
	*
FROM OrderProduct op WITH (NOLOCK)
JOIN Product p WITH (NOLOCK)
	ON op.ProductId = p.Id
WHERE op.OrderId = @OrderId";

                var results = db.QueryMultiple(sql, new { OrderId = 1 });

                order = results.Read<Order, Customer, Order>(
                    (o, c) =>
                        {
                            o.Customer = c;

                            return o;
                        },
                    splitOn: "Id").SingleOrDefault();

                if (order != null)
                {
                    order.Products = results.Read<Product>().ToList();
                }
            }

            this.textBox1.Text = JsonConvert.SerializeObject(order, Formatting.Indented);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var products = default(List<Product>);
            using (var db = new SqlConnection(ConnectionString))
            {
                var sql = @"
SELECT
	*
FROM Product p WITH (NOLOCK)";

                products = db.PolymorphicQuery<Product>(sql, "Type").ToList();
            }

            this.textBox1.Text = JsonConvert.SerializeObject(products, Formatting.Indented);
        }
    }
}