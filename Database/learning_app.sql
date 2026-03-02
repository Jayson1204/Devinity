-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: learning_app
-- ------------------------------------------------------
-- Server version	5.5.5-10.4.32-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `assessments`
--

DROP TABLE IF EXISTS `assessments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `assessments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) NOT NULL,
  `Level` varchar(50) DEFAULT NULL,
  `Category` varchar(255) DEFAULT NULL,
  `Question` text DEFAULT NULL,
  `StarterCode` text DEFAULT NULL,
  `ExpectedOutput` text DEFAULT NULL,
  `OrderIndex` int(11) DEFAULT 0,
  `CreatedAt` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `assessments`
--

LOCK TABLES `assessments` WRITE;
/*!40000 ALTER TABLE `assessments` DISABLE KEYS */;
INSERT INTO `assessments` VALUES (1,'Hello World in PHP','beginner','PHP','Write a PHP script that prints: Hello World','<?php\n// Write your code here\n','Hello World',1,'2026-02-28 20:55:08'),(2,'PHP Variables','beginner','PHP','Create a variable $name = \"PHP\" and print: Hello PHP','<?php\n$name = \"\";\n// Print Hello + name\n','Hello PHP',2,'2026-02-28 20:55:08'),(3,'PHP For Loop','intermediate','PHP','Use a for loop to print numbers 1 to 5 each on a new line','<?php\n// Write your loop here\n','1 2 3 4 5',3,'2026-02-28 20:55:08'),(4,'PHP Function','intermediate','PHP','Write a function add($a, $b) that returns the sum of two numbers and print add(3,5)','<?php\nfunction add($a, $b) {\n    // return sum\n}\necho add(3, 5);\n','8',4,'2026-02-28 20:55:08'),(5,'PHP Array Sum','advanced','PHP','Create an array [10, 20, 30] and print the sum of all elements','<?php\n$numbers = [10, 20, 30];\n// Print the sum\n','60',5,'2026-02-28 20:55:08'),(6,'Hello World in Python','beginner','Python','Write a Python program that prints: Hello World','print(\"\")','Hello World',1,'2026-02-28 20:55:08'),(7,'Sum Two Numbers','beginner','Python','Write a program that prints the sum of 2 and 3','result = print(result)','5',2,'2026-02-28 20:55:08'),(8,'Loop Challenge','intermediate','Python','Print numbers 1 to 5 using a for loop','for i in ...','1 2 3 4 5',3,'2026-02-28 20:55:08'),(9,'Python Function','intermediate','Python','Write a function add(a, b) that returns the sum and print add(4, 6)','def add(a, b):\n    # return sum\n\nprint(add(4, 6))','10',4,'2026-02-28 20:55:08'),(10,'Python List Sum','advanced','Python','Create a list [10, 20, 30] and print the sum of all elements','numbers = [10, 20, 30]\n# Print the sum','60',5,'2026-02-28 20:55:08'),(11,'Hello World in JavaScript','beginner','JavaScript','Write a JS program that prints: Hello World','// Write your code here','Hello World',1,'2026-02-28 20:55:08'),(12,'JS Variables','beginner','JavaScript','Create a variable name = \"JavaScript\" and print: Hello JavaScript','let name = \"\";\n// Print Hello + name','Hello JavaScript',2,'2026-02-28 20:55:08'),(13,'JS For Loop','intermediate','JavaScript','Use a for loop to print numbers 1 to 5 each on a new line','// Write your loop here','1 2 3 4 5',3,'2026-02-28 20:55:08'),(14,'JS Function','intermediate','JavaScript','Write a function add(a, b) that returns the sum and print add(3, 7)','function add(a, b) {\n    // return sum\n}\nconsole.log(add(3, 7));','10',4,'2026-02-28 20:55:08'),(15,'JS Array Sum','advanced','JavaScript','Create an array [10, 20, 30] and print the sum using reduce()','const numbers = [10, 20, 30];\n// Print the sum','60',5,'2026-02-28 20:55:08'),(16,'Hello World in Java','beginner','Java','Write a Java program that prints: Hello World','public class Main {\n    public static void main(String[] args) {\n        // Write your code here\n    }\n}','Hello World',1,'2026-02-28 20:55:08'),(17,'Java Variables','beginner','Java','Create a String variable name = \"Java\" and print: Hello Java','public class Main {\n    public static void main(String[] args) {\n        String name = \"\";\n        // Print Hello + name\n    }\n}','Hello Java',2,'2026-02-28 20:55:08'),(18,'Java For Loop','intermediate','Java','Use a for loop to print numbers 1 to 5 each on a new line','public class Main {\n    public static void main(String[] args) {\n        // Write your loop\n    }\n}','1 2 3 4 5',3,'2026-02-28 20:55:08'),(19,'Java Method','intermediate','Java','Write a method add(int a, int b) that returns the sum and print add(4, 6)','public class Main {\n    static int add(int a, int b) {\n        // return sum\n    }\n    public static void main(String[] args) {\n        System.out.println(add(4, 6));\n    }\n}','10',4,'2026-02-28 20:55:08'),(20,'Java Array Sum','advanced','Java','Create an int array {10, 20, 30} and print the sum of all elements','public class Main {\n    public static void main(String[] args) {\n        int[] numbers = {10, 20, 30};\n        // Print the sum\n    }\n}','60',5,'2026-02-28 20:55:08'),(21,'Hello World in C#','beginner','C#','Write a C# program that prints: Hello World','// Write your code here','Hello World',1,'2026-02-28 20:55:08'),(22,'C# Variables','beginner','C#','Create a string variable name = \"CSharp\" and print: Hello CSharp','string name = \"\";\n// Print Hello + name','Hello CSharp',2,'2026-02-28 20:55:08'),(23,'C# For Loop','intermediate','C#','Use a for loop to print numbers 1 to 5 each on a new line','// Write your loop here','1 2 3 4 5',3,'2026-02-28 20:55:08'),(24,'C# Method','intermediate','C#','Write a method Add(int a, int b) that returns the sum and print Add(5, 5)','static int Add(int a, int b) {\n    // return sum\n}\nConsole.WriteLine(Add(5, 5));','10',4,'2026-02-28 20:55:08'),(25,'C# Array Sum','advanced','C#','Create an int array {10, 20, 30} and print the sum using a loop','int[] numbers = {10, 20, 30};\n// Print the sum','60',5,'2026-02-28 20:55:08'),(26,'Hello World in C++','beginner','C++','Write a C++ program that prints: Hello World','#include <iostream>\nusing namespace std;\nint main() {\n    // Write your code here\n    return 0;\n}','Hello World',1,'2026-02-28 20:55:08'),(27,'C++ Variables','beginner','C++','Create a string variable name = \"CPP\" and print: Hello CPP','#include <iostream>\nusing namespace std;\nint main() {\n    string name = \"\";\n    // Print Hello + name\n    return 0;\n}','Hello CPP',2,'2026-02-28 20:55:08'),(28,'C++ For Loop','intermediate','C++','Use a for loop to print numbers 1 to 5 each on a new line','#include <iostream>\nusing namespace std;\nint main() {\n    // Write your loop\n    return 0;\n}','1 2 3 4 5',3,'2026-02-28 20:55:08'),(29,'C++ Function','intermediate','C++','Write a function add(int a, int b) that returns the sum and print add(3, 7)','#include <iostream>\nusing namespace std;\nint add(int a, int b) {\n    // return sum\n}\nint main() {\n    cout << add(3, 7);\n    return 0;\n}','10',4,'2026-02-28 20:55:08'),(30,'C++ Array Sum','advanced','C++','Create an array {10, 20, 30} and print the sum of all elements','#include <iostream>\nusing namespace std;\nint main() {\n    int numbers[] = {10, 20, 30};\n    // Print the sum\n    return 0;\n}','60',5,'2026-02-28 20:55:08'),(31,'Hello World in C','beginner','C','Write a C program that prints: Hello World','#include <stdio.h>\nint main() {\n    // Write your code here\n    return 0;\n}','Hello World',1,'2026-02-28 20:55:08'),(32,'C Variables','beginner','C','Create an int variable x = 42 and print it','#include <stdio.h>\nint main() {\n    int x = 0;\n    // Print x\n    return 0;\n}','42',2,'2026-02-28 20:55:08'),(33,'C For Loop','intermediate','C','Use a for loop to print numbers 1 to 5 each on a new line','#include <stdio.h>\nint main() {\n    // Write your loop\n    return 0;\n}','1 2 3 4 5',3,'2026-02-28 20:55:08'),(34,'C Function','intermediate','C','Write a function add(int a, int b) that returns the sum and print add(6, 4)','#include <stdio.h>\nint add(int a, int b) {\n    // return sum\n}\nint main() {\n    printf(\"%d\", add(6, 4));\n    return 0;\n}','10',4,'2026-02-28 20:55:08'),(35,'C Array Sum','advanced','C','Create an array {10, 20, 30} and print the sum of all elements','#include <stdio.h>\nint main() {\n    int numbers[] = {10, 20, 30};\n    // Print the sum\n    return 0;\n}','60',5,'2026-02-28 20:55:08'),(36,'SELECT Statement','beginner','MySQL','Write a query to select all columns from a table called \"users\"','-- Write your query here','SELECT * FROM users',1,'2026-02-28 20:55:08'),(37,'WHERE Clause','beginner','MySQL','Write a query to select all users where age > 18 from the \"users\" table','-- Write your query here','SELECT * FROM users WHERE age > 18',2,'2026-02-28 20:55:08'),(38,'ORDER BY','intermediate','MySQL','Write a query to select all users ordered by name in ascending order','-- Write your query here','SELECT * FROM users ORDER BY name ASC',3,'2026-02-28 20:55:08'),(39,'INSERT Statement','intermediate','MySQL','Write a query to insert a new user with name=\"John\" and age=25 into the \"users\" table','-- Write your query here','INSERT INTO users (name, age) VALUES (\'John\', 25)',4,'2026-02-28 20:55:08'),(40,'JOIN Query','advanced','MySQL','Write a query to join \"orders\" and \"users\" tables on users.id = orders.user_id and select all columns','-- Write your query here','SELECT * FROM orders JOIN users ON users.id = orders.user_id',5,'2026-02-28 20:55:08');
/*!40000 ALTER TABLE `assessments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `firebasevideos`
--

DROP TABLE IF EXISTS `firebasevideos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `firebasevideos` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) NOT NULL,
  `Description` text DEFAULT NULL,
  `FirebaseUrl` text NOT NULL,
  `ThumbnailUrl` text DEFAULT NULL,
  `Level` varchar(50) DEFAULT NULL,
  `Category` varchar(255) DEFAULT NULL,
  `Duration` varchar(20) DEFAULT NULL,
  `OrderIndex` int(11) DEFAULT 0,
  `CreatedAt` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `firebasevideos`
--

LOCK TABLES `firebasevideos` WRITE;
/*!40000 ALTER TABLE `firebasevideos` DISABLE KEYS */;
INSERT INTO `firebasevideos` VALUES (1,'C# Fundamentals','Learn C# basics','https://nipptmymnnsfjxoeotfv.supabase.co/storage/v1/object/public/videos/CSHARP.mp4','','beginner','C#','17:00',1,'2026-02-23 20:55:51'),(2,'C# Fundamentals','Learn C# basics','https://nipptmymnnsfjxoeotfv.supabase.co/storage/v1/object/public/videos/CSharp_beginner1.mp4','','beginner','C#','10:00',2,'2026-02-24 22:12:14'),(3,'C# Fundamentals','Learn C# basic','https://nipptmymnnsfjxoeotfv.supabase.co/storage/v1/object/public/videos/CSharp_beginner2.mp4','','beginner','C#','6:00',3,'2026-02-24 22:12:14'),(4,'C# Fundamentals','Learn C# basic','https://nipptmymnnsfjxoeotfv.supabase.co/storage/v1/object/public/videos/CSharp_beginner3.mp4','','beginner','C#','10:00',4,'2026-02-24 22:12:14'),(5,'C# Fundamentals','Learn C# basic','https://nipptmymnnsfjxoeotfv.supabase.co/storage/v1/object/public/videos/CSharp_inter3.mp4','','intermediate','C#','10:00',5,'2026-02-24 22:12:14'),(6,'C# Fundamentals','Learn C# basic','https://nipptmymnnsfjxoeotfv.supabase.co/storage/v1/object/public/videos/CSharp_inter4.mp4','','intermediate','C#','10:00',6,'2026-02-24 22:12:14'),(7,'C# Fundamentals','Learn C# basic','https://nipptmymnnsfjxoeotfv.supabase.co/storage/v1/object/public/videos/CSharp_inter5.mp4','','intermediate','C#','10:00',7,'2026-02-24 22:12:14');
/*!40000 ALTER TABLE `firebasevideos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `refresh_tokens`
--

DROP TABLE IF EXISTS `refresh_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `refresh_tokens` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` varchar(36) NOT NULL,
  `token` varchar(500) NOT NULL,
  `expires_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `revoked_at` timestamp NULL DEFAULT NULL,
  `replaced_by_token` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_token` (`token`(255)),
  KEY `idx_user_id` (`user_id`),
  CONSTRAINT `refresh_tokens_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=71 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `refresh_tokens`
--

LOCK TABLES `refresh_tokens` WRITE;
/*!40000 ALTER TABLE `refresh_tokens` DISABLE KEYS */;
INSERT INTO `refresh_tokens` VALUES (1,'abadb289-7a0c-453c-ba05-e741424ad9c1','RI1lIUUh0uAWjfQTD4QZmNFJ1maU2gnLqzrZL6pMa7FWppv3M8/M0rujiQROpKxVNRwcxuYKVE3HucT4cUi2iQ==','2026-02-22 18:21:07','2026-02-15 18:21:07',NULL,NULL),(2,'abadb289-7a0c-453c-ba05-e741424ad9c1','wnCrG+jt64KQ45xErrwSlMbWKbeIqaGTPevfw6eXP38/tCULdL6PzUQSFnDU1FVtrlbBFNbp02KejvR7H3wH2g==','2026-02-22 18:22:36','2026-02-15 18:22:36',NULL,NULL),(3,'abadb289-7a0c-453c-ba05-e741424ad9c1','UbAJOp4aLXPYHaYGY4dowYyDQdsESYyTZ7AJRzraesJgJ/AwAIN7zv1lwz0NLYhJkBzOY5jaCXvwSF+Kjih5/A==','2026-02-22 18:26:03','2026-02-15 18:26:03',NULL,NULL),(4,'abadb289-7a0c-453c-ba05-e741424ad9c1','anfvhWjyslcP1WmDc5d+Nh1ZEzqhMThI/xHyK8ZXUJennYRz8XPy8xM5GgXjvXv5DLj109n8AlmZCNxkkwUbAQ==','2026-02-22 18:26:55','2026-02-15 18:26:55',NULL,NULL),(5,'abadb289-7a0c-453c-ba05-e741424ad9c1','xIEcubdqftYZxxtPIjDSf29kexC4dcQZ6iHyaVaLRfIwsemWce0x9FdBcPdlupPYVGP/OD+AFW0Txg9VMQqRpQ==','2026-02-22 18:27:26','2026-02-15 18:27:26',NULL,NULL),(6,'abadb289-7a0c-453c-ba05-e741424ad9c1','VWk7GES2e/s0R6H2vbAGIBNc6S+Vago3vsLTHnjpBst/ZsjtGijVtLr9w5IcIpjrOy3+pOPoPKVH7VE5NPYn9w==','2026-02-22 18:34:06','2026-02-15 18:34:06',NULL,NULL),(7,'abadb289-7a0c-453c-ba05-e741424ad9c1','llDqtv8GyU7jYgCBW8WAXUmwX89xPuDazyOtYS1OgWHK+lXZp00iGuI66uMBco25gm2+V/82oje31p/D34MQJw==','2026-02-22 18:39:58','2026-02-15 18:39:58',NULL,NULL),(8,'abadb289-7a0c-453c-ba05-e741424ad9c1','gDdsymYusWpCCsoA+WdaiiR9H92IvFDbslRYBsh5Hal7l/09XzQlUJeYWR9xSRpBiFUZfSunQp6YKrkUoH/Vmw==','2026-02-23 04:22:13','2026-02-16 04:22:13',NULL,NULL),(9,'abadb289-7a0c-453c-ba05-e741424ad9c1','ShX+tff6ekJndtP0PexcFAPV+XgLUT+bIKGbqsouZx6xtQpppPJZQq/diCI/uUGCj3EkInC/p+mcQTtAlLzSEA==','2026-02-23 04:29:53','2026-02-16 04:29:53',NULL,NULL),(10,'abadb289-7a0c-453c-ba05-e741424ad9c1','4H5A3MtB8GvwxAebOe/3Vpg4LfXMIjOPeo10s0b2/JXNhB4L1icXr153Vtmicdnn9Ggg5yH6+P3r/KgUYwTbLQ==','2026-02-23 04:37:08','2026-02-16 04:37:08',NULL,NULL),(11,'abadb289-7a0c-453c-ba05-e741424ad9c1','TUMQhR9zxznURnl7Wi5XwBw2oDs1WRT8kaousxv+rNF7KE1x9t9YtnyGxZBw6TRMqmbLc6jGbyR+f0r3AOR8Xg==','2026-02-25 06:17:21','2026-02-18 06:17:21',NULL,NULL),(12,'abadb289-7a0c-453c-ba05-e741424ad9c1','OlK+MUT//ZrAvPn8Qr0UcDp54yzM0vco+renmi/qYhEvRPXWfXtO6H/bnex5D8l+YY5GvyeS7yJO4olxdPP86A==','2026-02-25 06:18:17','2026-02-18 06:18:17',NULL,NULL),(13,'abadb289-7a0c-453c-ba05-e741424ad9c1','O0+HicRzMlQ79z0NoG5B/4MzdJhiy/kCBagxPfu2ZTQwe+4/ja1f9RQ/l/ElUB4KygQRsa3YpqmJ+9vWi4WaQg==','2026-02-25 06:21:34','2026-02-18 06:21:34',NULL,NULL),(14,'abadb289-7a0c-453c-ba05-e741424ad9c1','DI7E1UnAQrgWRo9i5Nv4xQQ0ScdBNo+7wppgDrnONHF6XdN+FrPcJXiO8IETBz5iYbhHFEFs/1x6R0iacU1gLg==','2026-02-26 18:45:09','2026-02-19 18:45:09',NULL,NULL),(15,'abadb289-7a0c-453c-ba05-e741424ad9c1','QtWdySaK7J3GqJUtK8TEkWifNxSq9eMdWzOKHwRVzQYCV+2l/9Hew3PpEc2ZmRNf4M4exCrkPCgtNwUQqhRKbA==','2026-02-26 18:46:39','2026-02-19 18:46:39',NULL,NULL),(16,'abadb289-7a0c-453c-ba05-e741424ad9c1','P8X+svDI+kakY8WI7eT9Is5Quamv5XBMYh7QXF0SNBr5BdX24JovGfwYN04quhmZg3QRqMBllm+1jugUNrxhLQ==','2026-02-26 19:24:15','2026-02-19 19:24:15',NULL,NULL),(17,'abadb289-7a0c-453c-ba05-e741424ad9c1','NbdHAY5d0LFIS2EkdPAISRj+H4Mh/HR60RmcUN7+oGyUuRtrDOK9+8z6MBFiVDzmvB5V5Wcx2P5CSfshwc+daA==','2026-02-26 19:28:26','2026-02-19 19:28:26',NULL,NULL),(18,'abadb289-7a0c-453c-ba05-e741424ad9c1','KowRspQjGOSr331lYNew3em3P7eE5xnHvLHYMpiGr6sn6BX26jlW3dqcSWWfNrCMTm/xINMh3pOh5USZFbjMEg==','2026-02-26 19:32:23','2026-02-19 19:32:23',NULL,NULL),(19,'abadb289-7a0c-453c-ba05-e741424ad9c1','QHSNt6EgyvxKdrzAdzAycjDgHacsJz//z3ICkCU1nPxHT5iXo3qUd/a6B/5Zw73/Sq5xlz2bgZacPvJqH4Zrzg==','2026-02-27 05:15:35','2026-02-20 05:15:35',NULL,NULL),(20,'abadb289-7a0c-453c-ba05-e741424ad9c1','qQRvLGAMaIRqXe+17DJONbnLm5W8M7ApXVlsectlLTQEmDUWzRXOqPHN4xWFPZd6F94GZH+yn2QMNyBeyWSjCg==','2026-02-27 05:41:04','2026-02-20 05:41:04',NULL,NULL),(21,'abadb289-7a0c-453c-ba05-e741424ad9c1','gd7NlSNg/L37iWMqfzYS310q/LQ8GAwPy2RdiJDlx6O2db6UoYCZRj5IxNLB22SBWPLIo8isAIHMYB91QIDERw==','2026-02-27 05:59:04','2026-02-20 05:59:04',NULL,NULL),(22,'abadb289-7a0c-453c-ba05-e741424ad9c1','RPF5aPUF90lcOyi69Ew/MiBbMAKbBWql7bTgp+FFhAvqc2qMdNCIR3MtrACnUfsF5qXFf5Ap8J3LJVxs9/Qr3Q==','2026-02-27 06:08:13','2026-02-20 06:08:13',NULL,NULL),(23,'abadb289-7a0c-453c-ba05-e741424ad9c1','ftrkRSQrYO3xNI1GUo/x2e/fEOX7tM9f8Jc3wXl/hBUpndv0wmqb7zeH+jUUcISXNRE1VB/29ccXBlvmi5hOTw==','2026-02-27 06:09:56','2026-02-20 06:09:56',NULL,NULL),(24,'abadb289-7a0c-453c-ba05-e741424ad9c1','8I7ghdmSBKJWiY7O08y9SKdwBEYoFMI5iehrjuTzqottrsVuhOogHoPc2URaAhGhTXfBYgqeAxTDrZO54B+0dw==','2026-02-27 06:13:14','2026-02-20 06:13:14',NULL,NULL),(25,'abadb289-7a0c-453c-ba05-e741424ad9c1','CECxgwdTEmA5ytBaJ7uN6cyem5AUkraggxWaqEZ1wW70IR/HytdrSjHI39pop2xe7ol7qLGJKKgxHjZptfx8RQ==','2026-02-27 06:13:56','2026-02-20 06:13:56',NULL,NULL),(26,'abadb289-7a0c-453c-ba05-e741424ad9c1','brNR0DNEcWXf3mhLkmRCwrd42I1nCGuDcUDGVt/pMlInCTeLy4OuV9wMJXVOpxDWl4oyjK8BJUOinxU7ObMh4Q==','2026-02-27 06:19:35','2026-02-20 06:19:35',NULL,NULL),(27,'abadb289-7a0c-453c-ba05-e741424ad9c1','fx3Rh/MfWFCayKhCx/pdasdqFgU0nJ/Gp8w3pLD0m+oAsxpL4ojSX9MBYQWGi9Ytdi8ttOk3qP9zs/WubmodSQ==','2026-02-27 06:22:04','2026-02-20 06:22:04',NULL,NULL),(28,'abadb289-7a0c-453c-ba05-e741424ad9c1','O/SpPpCT751no/gl2JQfNHlqXEz0Q8Aga4yuAI8IOjXfAi5A6gOQgPYqsV/+Jrzr0tHP/PSKiP8Z2GbwXEhvhQ==','2026-02-27 06:25:47','2026-02-20 06:25:47',NULL,NULL),(29,'abadb289-7a0c-453c-ba05-e741424ad9c1','6QkdouVQVXIPYpVgmaM6rLy4auM0NGiQtf2G8b1YAodlwG101EQwtpWRmCB54AZTkB1AGM6NDj/2dQvcZkMm4g==','2026-02-27 06:27:41','2026-02-20 06:27:41',NULL,NULL),(30,'abadb289-7a0c-453c-ba05-e741424ad9c1','IMo+YQ1EuksnNX04/9jx9r5xGVa4EXtxDruYBjy0UlD/xMhu9EEU9wbByWqxgHk1tuv3ubM+kLGqpOrIuEkz6A==','2026-02-27 06:32:18','2026-02-20 06:32:18',NULL,NULL),(31,'abadb289-7a0c-453c-ba05-e741424ad9c1','NsvFjW0B7ScbDiSDf7j0JWUC4sCoX4VfGe3YzbpDzKUEV6hYPyBRN2o9bik8ztUytOZGrZduiA8+AqCbREUxFA==','2026-02-27 06:36:56','2026-02-20 06:36:56',NULL,NULL),(32,'abadb289-7a0c-453c-ba05-e741424ad9c1','sUa2DtHpegzWi4qUN7QdU3tTynu7JrxOVAA5lTDEK6y2T5hCeXK5F6Ptdzpwdy4YGqTk6UexaBNAisaZdriAoQ==','2026-03-02 05:06:52','2026-02-23 05:06:52',NULL,NULL),(33,'abadb289-7a0c-453c-ba05-e741424ad9c1','RyZb0qk80ZyRYWO2g1KGJmizYVIJYvjk7yKie7LBGkdUqvnHtAzIIIqERMGP26iONuZsnxZLuM80TBHZgL9ELA==','2026-03-02 05:14:26','2026-02-23 05:14:26',NULL,NULL),(34,'abadb289-7a0c-453c-ba05-e741424ad9c1','nWe0Fsr+CnRi1FzUiKleryGgadbQhuHhCodhQ3ROHjtpXf0hWPXNjdjDF5UonR4k31X0yiWduJu1UzwpfTHd9g==','2026-03-02 05:49:05','2026-02-23 05:49:05',NULL,NULL),(35,'abadb289-7a0c-453c-ba05-e741424ad9c1','0omAuUwVDgouiMzYwZm+DwxgCCGFw5zzX9VwDClHGA8h8CYlKXr0mtmRqikBuLa0riwk0TIAA1bbiLm9IYmH9Q==','2026-03-02 06:11:52','2026-02-23 06:11:52',NULL,NULL),(36,'abadb289-7a0c-453c-ba05-e741424ad9c1','pjaoHufwxcjlzpTZW33YkjMAuj9uc1MnuFAA8KulwYPxdn5mgvdVjNQ2MjYr0APam4fbTT2mNUBEY9/J5D5YFA==','2026-03-02 06:19:58','2026-02-23 06:19:58',NULL,NULL),(37,'abadb289-7a0c-453c-ba05-e741424ad9c1','dy8FoFd+3NXK9Z4FSrvthacNsIIqew/6PPzAkMgAysghd3Ihr0QriuX6yTmeSQO5ksnukKtK0ZYx4gS3SLiGyw==','2026-03-02 06:27:48','2026-02-23 06:27:48',NULL,NULL),(38,'abadb289-7a0c-453c-ba05-e741424ad9c1','54pYMvN+WUJUgY62F4jr507zszZ41m7xHgk2ARAGwlyjJlAAfGI9bNM/SG7XpsUJ+bF1b99EZ/lSKmzFhB4biw==','2026-03-02 06:39:24','2026-02-23 06:39:24',NULL,NULL),(39,'20d93d4c-0c4e-4bbe-bee1-f2593aa237f8','QSTIXAZ1w8lpFchB5Nqu0h3/wtsueu3GjFzok1zTqcdzmRGDAXP/r+Lx60Co4HJfuj1CQl4YqFclJzud7HwCig==','2026-03-02 06:49:46','2026-02-23 06:49:46',NULL,NULL),(40,'abadb289-7a0c-453c-ba05-e741424ad9c1','9r4PsTgOCcfVFt9Ah6ONu9KilfoXhFm1Snh9ZY45Gbo+GNbqGZUK8aKVN6MJtRLSvEquLPrGC72QFQpQkKEIXg==','2026-03-02 07:06:02','2026-02-23 07:06:02',NULL,NULL),(41,'abadb289-7a0c-453c-ba05-e741424ad9c1','0t4tTormPc4HfnBq+FmM6E4idkV74CmOOnRsLkxT6PYBXnVGcEXNXS0y9RoFHdDAX6RTsBHBc/pvqHJhsmbpLA==','2026-03-03 06:16:03','2026-02-24 06:16:03',NULL,NULL),(42,'abadb289-7a0c-453c-ba05-e741424ad9c1','MQbkdh2BUxdQGD1CMRZRrc5ztaSlVxJ3fo2iBsSVqtkgMuqPNw29eU1Pxiwgy2qxidyWsgftA1ogasAcTh+2ZQ==','2026-03-03 06:37:30','2026-02-24 06:37:30',NULL,NULL),(43,'abadb289-7a0c-453c-ba05-e741424ad9c1','76MX0H8IH5P+1VSyFbLsziEmd+Wh0GRuRysJmViQd/bIM3Zl72p3zPpTEd+dPiT0Ud2xbjmUINCwM99VQ2BkNg==','2026-03-03 06:49:58','2026-02-24 06:49:58',NULL,NULL),(44,'abadb289-7a0c-453c-ba05-e741424ad9c1','Bc7f4KE4fFae6+ICgfmEtCrM0sj7DYbexEdFiN+8Ms+7doZUsDV/Nyffd1oeXy3AzKQbDnlw+BMD8o0peaq9zw==','2026-03-03 06:55:36','2026-02-24 06:55:36',NULL,NULL),(45,'abadb289-7a0c-453c-ba05-e741424ad9c1','v19HdloVdlHoNKDkZUqptZufiv5Spw7tv7OXrSS6Ws7k6DUg1CW2MACUgUVwwcb10MoYItIpLFWyZ/l5aMzriA==','2026-03-03 07:09:47','2026-02-24 07:09:47',NULL,NULL),(46,'abadb289-7a0c-453c-ba05-e741424ad9c1','QbcYBfgvEKfuXi2Y5kw+FlCxZDnOJDQBmJoFtXg7atVQ6Q6vwzAe5MZG5SesVngbFvRPZIwKppb3mh7nfKTn5g==','2026-03-03 07:14:17','2026-02-24 07:14:17',NULL,NULL),(47,'abadb289-7a0c-453c-ba05-e741424ad9c1','4l8yc09UJITwH1ERhTljsi3M6Etc5o6wq95kgH20GrTKS/byQm9XwUEoWMoUwj3+8w4XQyyQv42yy+kM7sH62Q==','2026-03-03 07:20:55','2026-02-24 07:20:55',NULL,NULL),(48,'abadb289-7a0c-453c-ba05-e741424ad9c1','CU6KYk2R2DL/Q+6UPSTw5d/+C38HxWXwyrM9rC2Rngmr7kTnrhaS/9sCDxW5uIAPIFWuz1C0EAlMKB2cv2j/Xw==','2026-03-03 07:22:49','2026-02-24 07:22:49',NULL,NULL),(49,'abadb289-7a0c-453c-ba05-e741424ad9c1','Ki2hY1V6hlpvgT2NGZ67qFnto0BRbFRfKjmGos1DJCLEVU0LmXMjSr6OmZfGMHQV30LO4q8lOeI8CaF0woYmtA==','2026-03-03 07:26:31','2026-02-24 07:26:31',NULL,NULL),(50,'abadb289-7a0c-453c-ba05-e741424ad9c1','J84a+eO7REYKhiTSRs1rC0qLI/tjftJgNpYH6K/7Rb6mAEZIpRHbwSw+8v15mZm4yairj3eFQnul8dz7AgWz0Q==','2026-03-07 02:42:53','2026-02-28 02:42:53',NULL,NULL),(51,'abadb289-7a0c-453c-ba05-e741424ad9c1','3zQnaA6PamjxFgldElWNqZ4mFxoYSiLvREXOwWwvBnwiq5i3gUL9HIlBY8FhxZgNTZijq6WKEQAEaZelr7uBBw==','2026-03-07 02:49:24','2026-02-28 02:49:24',NULL,NULL),(52,'abadb289-7a0c-453c-ba05-e741424ad9c1','T6wv8CvZz1VJHeSV12x2I8EztH78jI3KBc3Cxl9Mls8cCvdhW8ERaYmwYi95ps0q7GCQ3/+ee6GCYeDqC/h5gw==','2026-03-07 02:59:25','2026-02-28 02:59:25',NULL,NULL),(53,'abadb289-7a0c-453c-ba05-e741424ad9c1','VShFWrbq3YTrjGdaJ84eVoFiAGLRXQwM6qRagnDAeZ/Cp5MNQd64pnurYf2OCyCD8B+hZgUkGil1GKaW6QK0gA==','2026-03-07 03:25:50','2026-02-28 03:25:50',NULL,NULL),(54,'abadb289-7a0c-453c-ba05-e741424ad9c1','vw2khDnHLlpYagsjIP/snHt/PDLDD7AETzfSU3VTp5aEdFYq5Ead6OeFzQ1I+YtoHCaKHeHtOEveRcdeRtdNWg==','2026-03-07 03:36:19','2026-02-28 03:36:19',NULL,NULL),(55,'abadb289-7a0c-453c-ba05-e741424ad9c1','aeidi3vkuvK2PGwS2SyAg9z8fW47NzS1OcN572cZPw1FQMaLULX/6M2gTx7t2q0z5xFmfo0tgjSGn/3pbkLYjg==','2026-03-07 03:49:08','2026-02-28 03:49:08',NULL,NULL),(56,'abadb289-7a0c-453c-ba05-e741424ad9c1','JA6eCXo4fefBck1lE9d+iWmTEu6+12tU1b1MSKPn2Geii2YFZG/ZeyuSH3TUGroM1faf4ZkRiTRCml0tSnyTiA==','2026-03-07 03:56:18','2026-02-28 03:56:18',NULL,NULL),(57,'abadb289-7a0c-453c-ba05-e741424ad9c1','ULnAONSdqZL5DVh36sC5b1SgBYyqt2v8Bl6x5/jopHcu/at+De79GoMFmxnbgdJBi5xe7sNZiCC8Zk8a/At2ZQ==','2026-03-07 04:19:38','2026-02-28 04:19:38',NULL,NULL),(58,'abadb289-7a0c-453c-ba05-e741424ad9c1','DEiiBCS4ddQgjw3qNMAJ3Xjr6mXlCcvXBFuWKcbRYuB4wZj0ZmSZ92JRGaHOr+PVT/JmyCsXGKIeqCs9rdukPQ==','2026-03-07 04:26:29','2026-02-28 04:26:29',NULL,NULL),(59,'abadb289-7a0c-453c-ba05-e741424ad9c1','aZ+JAD2P2VoiN9eF8J8uM/R8s17EoWMwIS0Gzd8YIOnF7/owOP6LwUxxyDaOlAQ6vahOfT0kB1y8dbHPmAi5Fw==','2026-03-07 04:26:52','2026-02-28 04:26:52',NULL,NULL),(60,'abadb289-7a0c-453c-ba05-e741424ad9c1','1tqXxThQODp8qLJUCZzI29zraAgF2DAf3Z7ThR8OdZw3fQUKkMKkl2f/6cxjsq2/7eKTxqWd1f8VQh9YwWNK/A==','2026-03-07 04:34:07','2026-02-28 04:34:07',NULL,NULL),(61,'abadb289-7a0c-453c-ba05-e741424ad9c1','3Pjnfc5Gf/qiemDFbSMbvYzVLaXAecvIUfvcvWSdK1AOSz0Ab0ZYCWOCA3XWy0RM45CF4uNI7M3RWxjcsEXShA==','2026-03-07 04:35:09','2026-02-28 04:35:09',NULL,NULL),(62,'abadb289-7a0c-453c-ba05-e741424ad9c1','ar27Iu9bKbglWeHjtXntwD2EzaeIb4zcm2KCKd8ZqpIkIRJFrnmh1ezde06gKNE3G5GbC7WgpHZ2xPqzRuhovg==','2026-02-28 12:39:23','2026-02-28 04:39:17','2026-02-28 04:39:23',NULL),(63,'abadb289-7a0c-453c-ba05-e741424ad9c1','oikw6Eo8hrlpsKPWAekSLQLNgxcvmIsw5W59yZO5/Ffu5wTNwi7mB1Ao6jP2vAA5WLBb1pchoxm23CBX3Jnw1w==','2026-02-28 12:41:05','2026-02-28 04:40:48','2026-02-28 04:41:05',NULL),(64,'abadb289-7a0c-453c-ba05-e741424ad9c1','jss/XroHgh7cVnqtXTRNQS+jXiCgKpx5G7vI8OP9LwfdL+opRtRX/b/P4zTIW68SWg3ajFdlMrzJvsry/fyBGQ==','2026-02-28 12:42:30','2026-02-28 04:42:01','2026-02-28 04:42:30',NULL),(65,'abadb289-7a0c-453c-ba05-e741424ad9c1','JqhK7pms1XKiXrtF/DDfRt4Ifg4usU92TSOaWoFKgc4iV8CzzhV1XbQ+cdgVi4YRFD1SxWUUa56x/6ExsEazjQ==','2026-02-28 12:45:24','2026-02-28 04:45:18','2026-02-28 04:45:24',NULL),(66,'abadb289-7a0c-453c-ba05-e741424ad9c1','TzujxLgcHod9zRxdFT007nZRVQoEVDGaV2vQE9awDX8UZZByzIFvYYxUexaXfLDRpNFYC5P7rdrvO0C44+blNw==','2026-03-07 04:45:41','2026-02-28 04:45:41',NULL,NULL),(67,'abadb289-7a0c-453c-ba05-e741424ad9c1','daGDlGpbVSV08KVXlQxhhkXRD0VjF4Urck0P92TWwQTU/tmlvmAx4rdAJgQMT/DvMnkIHpaR0B5cf27HGo9xDg==','2026-03-07 05:01:54','2026-02-28 05:01:54',NULL,NULL),(68,'abadb289-7a0c-453c-ba05-e741424ad9c1','Jc+StlHcTBmvmwsGd42QM8LaIDriHHzH6l9KCXwThc7peWhaPzbNf5UT+jpi9dotRZmVNOlgE+Md89LxNVfuxQ==','2026-03-08 06:52:44','2026-03-01 06:52:44',NULL,NULL),(69,'abadb289-7a0c-453c-ba05-e741424ad9c1','fXywCQrxaFw9jlEAuUS/nlIe5PrseWJP2HLY+zSAGv7waEz/fn9XwGSu44SyfJH2RREWO6NxL4yYGJUQNekztg==','2026-03-08 06:57:35','2026-03-01 06:57:35',NULL,NULL),(70,'abadb289-7a0c-453c-ba05-e741424ad9c1','pGpXJPUi+lueb3ZdzZZlKfOQkNMAidGgjPxaCf3Xn1jaVh+iFu76rlMZiXCNj1Dd8dpBR5dU0zieVj2LBc/Wqw==','2026-03-08 07:11:18','2026-03-01 07:11:18',NULL,NULL);
/*!40000 ALTER TABLE `refresh_tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_progress`
--

DROP TABLE IF EXISTS `user_progress`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_progress` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` varchar(36) NOT NULL,
  `AssessmentId` int(11) NOT NULL,
  `Category` varchar(255) DEFAULT NULL,
  `Level` varchar(50) DEFAULT NULL,
  `IsCompleted` tinyint(1) DEFAULT 0,
  `Score` int(11) DEFAULT 0,
  `CompletedAt` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE KEY `unique_user_assessment` (`UserId`,`AssessmentId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_progress`
--

LOCK TABLES `user_progress` WRITE;
/*!40000 ALTER TABLE `user_progress` DISABLE KEYS */;
INSERT INTO `user_progress` VALUES (1,'abadb289-7a0c-453c-ba05-e741424ad9c1',17,'C#','beginner',1,10,'2026-02-24 15:15:19'),(2,'abadb289-7a0c-453c-ba05-e741424ad9c1',21,'C#','beginner',1,10,'2026-02-28 13:07:27'),(3,'abadb289-7a0c-453c-ba05-e741424ad9c1',22,'C#','beginner',1,10,'2026-03-01 14:53:45');
/*!40000 ALTER TABLE `user_progress` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` varchar(36) NOT NULL,
  `email` varchar(255) NOT NULL,
  `full_name` varchar(255) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `email_verified` tinyint(1) DEFAULT 0,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `last_login` timestamp NULL DEFAULT NULL,
  `is_active` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`),
  KEY `idx_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES ('1633e7a6-0903-4e8d-8045-e868ebccdb00','jaysonevangelista@gmail.com','Jayson Evangelistsa','$2a$11$tWAXPCkG.I6/RobIA70rGuJbRIvxNvWw/izQlGAsC4is6AXQk6MSu',0,'2026-02-15 18:07:02','2026-02-15 18:07:02',NULL,1),('20d93d4c-0c4e-4bbe-bee1-f2593aa237f8','g@gmail.com','gabriel martinez','$2a$11$yKwQZmA7o9UPOqDcJd5rY./WNpMYouDKdf5Y1M9aHxG8sr6iAUQUW',0,'2026-02-23 06:48:53','2026-02-23 14:49:46','2026-02-23 06:49:46',1),('abadb289-7a0c-453c-ba05-e741424ad9c1','jaysonevangelista111@gmail.com','jayson ','$2a$11$RsVwjP3VxZ0cLM1yEq5yS.MLnsjebHCVbtE1qJbceeQ.inu.RfkcO',0,'2026-02-15 18:14:13','2026-03-01 15:11:18','2026-03-01 07:11:18',1);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `video_progress`
--

DROP TABLE IF EXISTS `video_progress`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `video_progress` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` varchar(36) NOT NULL,
  `VideoId` int(11) NOT NULL,
  `Category` varchar(255) DEFAULT NULL,
  `IsWatched` tinyint(1) DEFAULT 0,
  `WatchedAt` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`),
  UNIQUE KEY `unique_user_video` (`UserId`,`VideoId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `video_progress`
--

LOCK TABLES `video_progress` WRITE;
/*!40000 ALTER TABLE `video_progress` DISABLE KEYS */;
INSERT INTO `video_progress` VALUES (1,'abadb289-7a0c-453c-ba05-e741424ad9c1',1,'C#',1,'2026-02-28 12:56:47');
/*!40000 ALTER TABLE `video_progress` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'learning_app'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-01 23:22:12
