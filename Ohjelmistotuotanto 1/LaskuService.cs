using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Ohjelmistotuotanto_1
{
    // Service class for handling invoice operations
    public class LaskuService
    {
        private DatabaseHelper _dbHelper;

        public LaskuService()
        {
            _dbHelper = new DatabaseHelper();
        }

        // Get all invoices with detailed information
        public async Task<List<Lasku>> GetLaskutAsync()
        {
            try
            {
                // SQL query to get invoice data joined with related tables
                string query = @"
                    SELECT l.lasku_id, l.varaus_id, l.summa, l.alv, l.maksettu,
                           a.etunimi, a.sukunimi, 
                           m.mokkinimi,
                           v.varattu_alkupvm, v.varattu_loppupvm
                    FROM lasku l
                    JOIN varaus v ON l.varaus_id = v.varaus_id
                    JOIN asiakas a ON v.asiakas_id = a.asiakas_id
                    JOIN mokki m ON v.mokki_id = m.mokki_id
                    ORDER BY l.lasku_id DESC";

                DataTable result = await _dbHelper.GetDataAsync(query);
                List<Lasku> laskut = new List<Lasku>();

                foreach (DataRow row in result.Rows)
                {
                    Lasku lasku = new Lasku
                    {
                        LaskuId = Convert.ToInt32(row["lasku_id"]),
                        VarausId = Convert.ToInt32(row["varaus_id"]),
                        Summa = Convert.ToDouble(row["summa"]),
                        Alv = Convert.ToDouble(row["alv"]),
                        Maksettu = Convert.ToBoolean(row["maksettu"]),
                        AsiakkaanNimi = $"{row["etunimi"]} {row["sukunimi"]}",
                        MokinNimi = row["mokkinimi"].ToString(),
                        AlkuPvm = Convert.ToDateTime(row["varattu_alkupvm"]),
                        LoppuPvm = Convert.ToDateTime(row["varattu_loppupvm"]),
                    };
                    laskut.Add(lasku);
                }

                return laskut;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting invoices: {ex.Message}");
                throw;
            }
        }

        // Get a single invoice by ID
        public async Task<Lasku> GetLaskuByIdAsync(int laskuId)
        {
            try
            {
                string query = @"
                    SELECT l.lasku_id, l.varaus_id, l.summa, l.alv, l.maksettu,
                           a.etunimi, a.sukunimi, 
                           m.mokkinimi,
                           v.varattu_alkupvm, v.varattu_loppupvm
                    FROM lasku l
                    JOIN varaus v ON l.varaus_id = v.varaus_id
                    JOIN asiakas a ON v.asiakas_id = a.asiakas_id
                    JOIN mokki m ON v.mokki_id = m.mokki_id
                    WHERE l.lasku_id = @laskuId";

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@laskuId", laskuId }
                };

                DataTable result = await _dbHelper.GetDataWithParamsAsync(query, parameters);
                
                if (result.Rows.Count == 0)
                    return null;

                DataRow row = result.Rows[0];
                Lasku lasku = new Lasku
                {
                    LaskuId = Convert.ToInt32(row["lasku_id"]),
                    VarausId = Convert.ToInt32(row["varaus_id"]),
                    Summa = Convert.ToDouble(row["summa"]),
                    Alv = Convert.ToDouble(row["alv"]),
                    Maksettu = Convert.ToBoolean(row["maksettu"]),
                    AsiakkaanNimi = $"{row["etunimi"]} {row["sukunimi"]}",
                    MokinNimi = row["mokkinimi"].ToString(),
                    AlkuPvm = Convert.ToDateTime(row["varattu_alkupvm"]),
                    LoppuPvm = Convert.ToDateTime(row["varattu_loppupvm"]),
                };

                return lasku;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting invoice: {ex.Message}");
                throw;
            }
        }

        // Create a new invoice for a reservation
        public async Task<int> CreateLaskuAsync(int varausId, double summa, double alv)
        {
            try
            {
                // Get next available invoice ID
                string getMaxIdQuery = "SELECT COALESCE(MAX(lasku_id), 0) + 1 FROM lasku";
                object maxIdResult = await _dbHelper.ExecuteScalarAsync(getMaxIdQuery);
                int newLaskuId = Convert.ToInt32(maxIdResult);

                // Insert new invoice
                string insertQuery = @"
                    INSERT INTO lasku (lasku_id, varaus_id, summa, alv, maksettu)
                    VALUES (@laskuId, @varausId, @summa, @alv, @maksettu)";

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@laskuId", newLaskuId },
                    { "@varausId", varausId },
                    { "@summa", summa },
                    { "@alv", alv },
                    { "@maksettu", false }
                };

                await _dbHelper.ExecuteNonQueryAsync(insertQuery, parameters);
                return newLaskuId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating invoice: {ex.Message}");
                throw;
            }
        }

        // Delete an invoice
        public async Task DeleteLaskuAsync(int laskuId)
        {
            try
            {
                string query = "DELETE FROM lasku WHERE lasku_id = @laskuId";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@laskuId", laskuId }
                };

                await _dbHelper.ExecuteNonQueryAsync(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting invoice: {ex.Message}");
                throw;
            }
        }

        // Mark an invoice as paid
        public async Task MarkAsPaidAsync(int laskuId)
        {
            try
            {
                string query = "UPDATE lasku SET maksettu = 1 WHERE lasku_id = @laskuId";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@laskuId", laskuId }
                };

                await _dbHelper.ExecuteNonQueryAsync(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking invoice as paid: {ex.Message}");
                throw;
            }
        }

        // Get all unpaid reservations without an invoice
        public async Task<DataTable> GetReservationsWithoutInvoicesAsync()
        {
            try
            {
                string query = @"
                    SELECT v.varaus_id, a.etunimi, a.sukunimi, m.mokkinimi, 
                           v.varattu_alkupvm, v.varattu_loppupvm,
                           m.hinta AS mokki_hinta,
                           DATEDIFF(v.varattu_loppupvm, v.varattu_alkupvm) AS nights
                    FROM varaus v
                    JOIN asiakas a ON v.asiakas_id = a.asiakas_id
                    JOIN mokki m ON v.mokki_id = m.mokki_id
                    LEFT JOIN lasku l ON v.varaus_id = l.varaus_id
                    WHERE l.lasku_id IS NULL
                    ORDER BY v.varattu_alkupvm DESC";

                return await _dbHelper.GetDataAsync(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting reservations without invoices: {ex.Message}");
                throw;
            }
        }
    }
} 