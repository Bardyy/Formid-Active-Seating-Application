<?php 

$con = mysqli_connect('localhost','root','root');

// Check connection
if(mysqli_connect_errno()){
  echo "1";
  exit();
}

// Create database
$sql_create_db = "CREATE DATABASE FormidActiveSeating";
mysqli_query($con, $sql_create_db) or die("1: Failed to create database");

// Create User table
$sql_user = "CREATE TABLE User (username VARCHAR(50) PRIMARY KEY NOT NULL, pass VARCHAR(50)  NOT NULL, firstName VARCHAR(50) NOT NULL, lastName VARCHAR(50) NOT NULL, email VARCHAR(50) NOT NULL);"
mysqli_query($con, $sql_user) or die("2: Failed to create User table");

// // Create User table
// $sql_user = "CREATE TABLE User (username VARCHAR(50) PRIMARY KEY NOT NULL, pass VARCHAR(50)  NOT NULL, firstName VARCHAR(50) NOT NULL, lastName VARCHAR(50) NOT NULL, email VARCHAR(50) NOT NULL);"
// if ($con->query($sql_user) === TRUE) {
//   echo "Database created successfully";
// } else {
//   echo "Error creating database: " . $con->error;
// }

$con->close();

echo("0");

?>