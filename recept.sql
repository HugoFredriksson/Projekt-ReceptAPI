-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Värd: 127.0.0.1
-- Tid vid skapande: 16 feb 2024 kl 15:02
-- Serverversion: 10.4.28-MariaDB
-- PHP-version: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Databas: `recept`
--

-- --------------------------------------------------------

--
-- Tabellstruktur `category`
--

CREATE TABLE `category` (
  `Id` int(32) NOT NULL,
  `RecipeId` int(20) NOT NULL,
  `Category` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumpning av Data i tabell `category`
--

INSERT INTO `category` (`Id`, `RecipeId`, `Category`) VALUES
(1, 3, 'Övrigt');

-- --------------------------------------------------------

--
-- Tabellstruktur `comment`
--

CREATE TABLE `comment` (
  `CommentId` int(32) NOT NULL,
  `UserId` int(32) NOT NULL,
  `RecipeId` int(32) NOT NULL,
  `TimeStamp` timestamp(6) NOT NULL DEFAULT current_timestamp(6) ON UPDATE current_timestamp(6),
  `Content` varchar(300) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumpning av Data i tabell `comment`
--

INSERT INTO `comment` (`CommentId`, `UserId`, `RecipeId`, `TimeStamp`, `Content`) VALUES
(1, 1, 3, '2024-02-13 09:25:36.547458', 'Hej, det här är inte ett recept, sluta sprida lögner :-/'),
(2, 1, 3, '2024-02-13 06:31:39.686825', 'Hej, det här är inte ett recept, sluta sprida lögner'),
(3, 2, 3, '2024-02-07 04:15:46.399131', 'Test');

-- --------------------------------------------------------

--
-- Tabellstruktur `likes`
--

CREATE TABLE `likes` (
  `RecipeId` int(32) NOT NULL,
  `UserId` int(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Tabellstruktur `recipe`
--

CREATE TABLE `recipe` (
  `Id` int(20) NOT NULL,
  `UserId` int(32) NOT NULL,
  `UserName` varchar(20) NOT NULL,
  `Title` varchar(100) NOT NULL,
  `Description` varchar(200) NOT NULL,
  `ImageUrl` varchar(32) NOT NULL,
  `TimeStamp` varchar(32) NOT NULL DEFAULT current_timestamp(),
  `Content` varchar(1000) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumpning av Data i tabell `recipe`
--

INSERT INTO `recipe` (`Id`, `UserId`, `UserName`, `Title`, `Description`, `ImageUrl`, `TimeStamp`, `Content`) VALUES
(3, 1, 'Test', 'test', 'test', 'test', 'current_timestamp()', 'test'),
(4, 1, '2', '2', '2', '322', '2024-02-16 11:56:56', '32');

-- --------------------------------------------------------

--
-- Tabellstruktur `user`
--

CREATE TABLE `user` (
  `id` int(32) NOT NULL,
  `UserName` varchar(20) NOT NULL,
  `Email` varchar(32) NOT NULL,
  `Password` varchar(32) NOT NULL,
  `Role` int(2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumpning av Data i tabell `user`
--

INSERT INTO `user` (`id`, `UserName`, `Email`, `Password`, `Role`) VALUES
(1, 'Admin', 'Admin@Admin.com', '123', 3),
(2, 'Testmannen', 'Mannentest@test.com', '123', 2),
(3, 'test', 'test', 'test', 2),
(4, 'testnamn', 'testmail', '$2a$11$fjI3Bxt/Ijc3DT06ikVnJ.Ag.', 2);

--
-- Index för dumpade tabeller
--

--
-- Index för tabell `category`
--
ALTER TABLE `category`
  ADD PRIMARY KEY (`Id`);

--
-- Index för tabell `comment`
--
ALTER TABLE `comment`
  ADD PRIMARY KEY (`CommentId`);

--
-- Index för tabell `likes`
--
ALTER TABLE `likes`
  ADD PRIMARY KEY (`RecipeId`);

--
-- Index för tabell `recipe`
--
ALTER TABLE `recipe`
  ADD PRIMARY KEY (`Id`);

--
-- Index för tabell `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT för dumpade tabeller
--

--
-- AUTO_INCREMENT för tabell `category`
--
ALTER TABLE `category`
  MODIFY `Id` int(32) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT för tabell `comment`
--
ALTER TABLE `comment`
  MODIFY `CommentId` int(32) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT för tabell `likes`
--
ALTER TABLE `likes`
  MODIFY `RecipeId` int(32) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT för tabell `recipe`
--
ALTER TABLE `recipe`
  MODIFY `Id` int(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT för tabell `user`
--
ALTER TABLE `user`
  MODIFY `id` int(32) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
