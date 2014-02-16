# Insert rows into the Pictures table by reading the list of pictures from a folder (for populating test data)

$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString = "Data Source=.\sqlexpress;Initial Catalog=nietoyosten2-dev;Integrated Security=True"

$conn.Open()

$files = gci "D:\skydrive\Pictures\camino de santiago\*.jpg"

$files | % {
  $sql = "INSERT INTO Pictures(AlbumID,Title,FileName) VALUES (2,'{0}','{0}')" -f $_.Name
  $cmd = New-Object System.Data.SqlClient.SqlCommand -ArgumentList $sql, $conn
  $cmd.ExecuteNonQuery();
}

$conn.Close();