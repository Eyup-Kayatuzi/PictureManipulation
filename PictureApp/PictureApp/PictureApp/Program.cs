using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Text.Json;

namespace PictureApp
{
	class Program
	{
		static string _connectionString;

		static void Main(string[] args)
		{
			
			_connectionString = (JsonSerializer.Deserialize<ConnectionSettings>(File.ReadAllText( "../../../appSettings.json"))).connectionString;
			string processType = null;
			bool isContinue = true;
			do
			{
				Console.Write("Please enter U/u for update, R/r for read  adn A/a for add on DataBase and E/e for end to process:");
				processType = Console.ReadLine();
				if (processType.ToLower() == "u")
				{
					UpdateDb();

				}
				else if (processType.ToLower() == "r")
				{
					ReadDb();
				}
				else if (processType.ToLower() == "a")
				{
					AddPictureToVpBlobDb();
				}
				else if (processType.ToLower() == "t")
				{
					isContinue = false;
				}
				else
				{
					Console.WriteLine("Invalid key entered");
				}

			} while (isContinue);

			#region Version1
			/*
             Writing to db
             */
			//try
			//{
			//	Console.Write("Please enter the file path of the image:");
			//	string filePath = Console.ReadLine();
			//	Console.Write("Please enter target ID:");
			//	int imageID = int.Parse(Console.ReadLine());
			//	if (File.Exists(filePath))
			//	{
			//		byte[] imageBytes = File.ReadAllBytes(filePath);
			//		string connectionString = "Server=ISTTURSQL-P01\\VERIBRANCH;Database=Vc8ProductIB.NetCore.NewUIDemo;User ID=VeriBranchUser;Password=VeriBranchUser123;MultipleActiveResultSets=True;TrustServerCertificate=True";
			//		using (SqlConnection connection = new SqlConnection(connectionString))
			//		{
			//			connection.Open();
			//			string query = "Update VpSecurityImage Set Value = @ImageData Where ID = @imageDataId";
			//			using (SqlCommand command = new SqlCommand(query, connection))
			//			{
			//				command.Parameters.AddWithValue("@ImageData", imageBytes);
			//				command.Parameters.AddWithValue("@imageDataId", imageID);
			//				var numberOfAffectedRows = command.ExecuteNonQuery();
			//				Console.WriteLine($"{numberOfAffectedRows} row affected on update process");
			//			}
			//		}
			//	}
			//	else
			//	{
			//		Console.WriteLine("The specified file where entered file path is not found.");
			//	}
			//}
			//catch (Exception e)
			//{
			//	Console.WriteLine(e.Message);
			//}

			/*
             Reading from db
             */
			//try
			//{
			//	Console.Write("Please enter picture ID:");
			//	int imageID = int.Parse(Console.ReadLine());
			//	string connectionString = "Server=ISTTURSQL-P01\\VERIBRANCH;Database=Vc8ProductIB.NetCore.NewUIDemo;User ID=VeriBranchUser;Password=VeriBranchUser123;MultipleActiveResultSets=True;TrustServerCertificate=True";
			//	string query = "SELECT Value FROM VpSecurityImage WHERE ID = @ID";
			//	using (SqlConnection connection = new SqlConnection(connectionString))
			//	{
			//		connection.Open();
			//		using (SqlCommand command = new SqlCommand(query, connection))
			//		{
			//			command.Parameters.AddWithValue("@ID", imageID);
			//			byte[] imageData = (byte[])command.ExecuteScalar();
			//			if (imageData != null && imageData.Length > 0)
			//			{
			//				string base64String = Convert.ToBase64String(imageData);
			//				Console.WriteLine($"Successfully read : {base64String}");
			//			}
			//			else
			//			{
			//				Console.WriteLine("The picture where entered id is not found");
			//			}
			//		}
			//	}
			//}
			//catch (Exception e)
			//{
			//	Console.WriteLine(e.Message);
			//}
			#endregion

		}

		static void AddPictureToVpBlobDb()
		{
			try
			{
				Console.Write("Please enter the file path of the image(with extension part as test.png):");
				string filePath = Console.ReadLine();
				Console.Write("Please enter the file name of the image:");
				string fileName = Console.ReadLine();
				Console.Write("Please enter the file extension(.png or etc.) of the image:");
				string fileExtension = Console.ReadLine();
				Console.Write("Please enter the file Size of the image:");
				int fileSize = Convert.ToInt32(Console.ReadLine());
				if (File.Exists(filePath))
				{
					byte[] imageBytes = File.ReadAllBytes(filePath);
					using (SqlConnection connection = new SqlConnection(_connectionString))
					{
						connection.Open();
						string query = "Insert into VpBlob (FileName, FileExtension, Size, Data) VALUES (@fileName, @fileExtension, @fileSize, @imageBytes)";
						using (SqlCommand command = new SqlCommand(query, connection))
						{
							command.Parameters.AddWithValue("@fileName", fileName);
							command.Parameters.AddWithValue("@fileExtension", fileExtension);
							command.Parameters.AddWithValue("@fileSize", fileSize);
							command.Parameters.AddWithValue("@imageBytes", imageBytes);
							var numberOfAffectedRows = command.ExecuteNonQuery();
							Console.WriteLine($"{numberOfAffectedRows} row affected on add process");
						}
					}
				}
				else
				{
					Console.WriteLine("The specified file where entered file path is not found.");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		static void UpdateDb()
		{
			try
			{
				Console.Write("Please enter the file path of the image(with extension part as test.png):");
				string filePath = Console.ReadLine();
				Console.Write("Please enter target ID:");
				int imageID = int.Parse(Console.ReadLine());
				if (File.Exists(filePath))
				{
					byte[] imageBytes = File.ReadAllBytes(filePath);
					using (SqlConnection connection = new SqlConnection(_connectionString))
					{
						connection.Open();
						string query = "Update VpSecurityImage Set Value = @ImageData Where ID = @imageDataId";
						using (SqlCommand command = new SqlCommand(query, connection))
						{
							command.Parameters.AddWithValue("@ImageData", imageBytes);
							command.Parameters.AddWithValue("@imageDataId", imageID);
							var numberOfAffectedRows = command.ExecuteNonQuery();
							Console.WriteLine($"{numberOfAffectedRows} row affected on update process");
						}
					}
				}
				else
				{
					Console.WriteLine("The specified file where entered file path is not found.");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		static void ReadDb()
		{
			try
			{
				Console.Write("Please enter picture ID:");
				int imageID = int.Parse(Console.ReadLine());
				string query = "SELECT Value FROM VpSecurityImage WHERE ID = @ID";
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@ID", imageID);
						byte[] imageData = (byte[])command.ExecuteScalar();
						if (imageData != null && imageData.Length > 0)
						{
							string base64String = Convert.ToBase64String(imageData);
							Console.WriteLine($"Successfully read : {base64String}");
						}
						else
						{
							Console.WriteLine("The picture where entered id is not found");
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
