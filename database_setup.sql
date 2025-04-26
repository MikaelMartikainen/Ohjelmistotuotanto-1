-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema vn
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `vn` ;

-- -----------------------------------------------------
-- Schema vn
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `vn` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_bin ;
USE `vn` ;

-- -----------------------------------------------------
-- Table `vn`.`alue`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`alue` ;

CREATE TABLE IF NOT EXISTS `vn`.`alue` (
  `alue_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `nimi` VARCHAR(40) NULL DEFAULT NULL,
  PRIMARY KEY (`alue_id`),
  INDEX `alue_nimi_index` (`nimi` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`posti`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`posti` ;

CREATE TABLE IF NOT EXISTS `vn`.`posti` (
  `postinro` CHAR(5) NOT NULL,
  `toimipaikka` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`postinro`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`asiakas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`asiakas` ;

CREATE TABLE IF NOT EXISTS `vn`.`asiakas` (
  `asiakas_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `postinro` CHAR(5) NOT NULL,
  `etunimi` VARCHAR(20) NULL DEFAULT NULL,
  `sukunimi` VARCHAR(40) NULL DEFAULT NULL,
  `lahiosoite` VARCHAR(40) NULL DEFAULT NULL,
  `email` VARCHAR(50) NULL DEFAULT NULL,
  `puhelinnro` VARCHAR(15) NULL DEFAULT NULL,
  PRIMARY KEY (`asiakas_id`),
  INDEX `fk_as_posti1_idx` (`postinro` ASC) VISIBLE,
  INDEX `asiakas_snimi_idx` (`sukunimi` ASC) VISIBLE,
  INDEX `asiakas_enimi_idx` (`etunimi` ASC) VISIBLE,
  CONSTRAINT `fk_asiakas_posti`
    FOREIGN KEY (`postinro`)
    REFERENCES `vn`.`posti` (`postinro`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`mokki`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`mokki` ;

CREATE TABLE IF NOT EXISTS `vn`.`mokki` (
  `mokki_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `alue_id` INT UNSIGNED NOT NULL,
  `postinro` CHAR(5) NOT NULL,
  `mokkinimi` VARCHAR(45) NULL DEFAULT NULL,
  `katuosoite` VARCHAR(45) NULL DEFAULT NULL,
  `hinta` DOUBLE(8,2) NOT NULL,
  `kuvaus` VARCHAR(150) NULL DEFAULT NULL,
  `henkilomaara` INT NULL DEFAULT NULL,
  `varustelu` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`mokki_id`),
  INDEX `fk_mokki_alue_idx` (`alue_id` ASC) VISIBLE,
  INDEX `fk_mokki_posti_idx` (`postinro` ASC) VISIBLE,
  CONSTRAINT `fk_mokki_alue`
    FOREIGN KEY (`alue_id`)
    REFERENCES `vn`.`alue` (`alue_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_mokki_posti`
    FOREIGN KEY (`postinro`)
    REFERENCES `vn`.`posti` (`postinro`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`varaus`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`varaus` ;

CREATE TABLE IF NOT EXISTS `vn`.`varaus` (
  `varaus_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `asiakas_id` INT UNSIGNED NOT NULL,
  `mokki_id` INT UNSIGNED NOT NULL,
  `varattu_pvm` DATETIME NULL DEFAULT NULL,
  `vahvistus_pvm` DATETIME NULL DEFAULT NULL,
  `varattu_alkupvm` DATETIME NULL DEFAULT NULL,
  `varattu_loppupvm` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`varaus_id`),
  INDEX `varaus_as_id_index` (`asiakas_id` ASC) VISIBLE,
  INDEX `fk_var_mok_idx` (`mokki_id` ASC) VISIBLE,
  CONSTRAINT `fk_varaus_mokki`
    FOREIGN KEY (`mokki_id`)
    REFERENCES `vn`.`mokki` (`mokki_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `varaus_ibfk`
    FOREIGN KEY (`asiakas_id`)
    REFERENCES `vn`.`asiakas` (`asiakas_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`lasku`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`lasku` ;

CREATE TABLE IF NOT EXISTS `vn`.`lasku` (
  `lasku_id` INT NOT NULL,
  `varaus_id` INT UNSIGNED NOT NULL,
  `summa` DOUBLE(8,2) NOT NULL,
  `alv` DOUBLE(8,2) NOT NULL,
  `maksettu` TINYINT NOT NULL DEFAULT 0,
  PRIMARY KEY (`lasku_id`),
  INDEX `lasku_ibfk_1` (`varaus_id` ASC) VISIBLE,
  CONSTRAINT `lasku_ibfk_1`
    FOREIGN KEY (`varaus_id`)
    REFERENCES `vn`.`varaus` (`varaus_id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`palvelu`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`palvelu` ;

CREATE TABLE IF NOT EXISTS `vn`.`palvelu` (
  `palvelu_id` INT UNSIGNED NOT NULL,
  `alue_id` INT UNSIGNED NOT NULL,
  `nimi` VARCHAR(40) NULL DEFAULT NULL,
  `kuvaus` VARCHAR(255) NULL DEFAULT NULL,
  `hinta` DOUBLE(8,2) NOT NULL,
  `alv` DOUBLE(8,2) NOT NULL,
  PRIMARY KEY (`palvelu_id`),
  INDEX `Palvelu_nimi_index` (`nimi` ASC) VISIBLE,
  INDEX `palv_toimip_id_ind` (`alue_id` ASC) VISIBLE,
  CONSTRAINT `palvelu_ibfk_1`
    FOREIGN KEY (`alue_id`)
    REFERENCES `vn`.`alue` (`alue_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`varauksen_palvelut`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`varauksen_palvelut` ;

CREATE TABLE IF NOT EXISTS `vn`.`varauksen_palvelut` (
  `varaus_id` INT UNSIGNED NOT NULL,
  `palvelu_id` INT UNSIGNED NOT NULL,
  `lkm` INT NOT NULL,
  PRIMARY KEY (`palvelu_id`, `varaus_id`),
  INDEX `vp_varaus_id_index` (`varaus_id` ASC) VISIBLE,
  INDEX `vp_palvelu_id_index` (`palvelu_id` ASC) VISIBLE,
  CONSTRAINT `fk_palvelu`
    FOREIGN KEY (`palvelu_id`)
    REFERENCES `vn`.`palvelu` (`palvelu_id`),
  CONSTRAINT `fk_varaus`
    FOREIGN KEY (`varaus_id`)
    REFERENCES `vn`.`varaus` (`varaus_id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

-- Add sample data
-- First add postal code data
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES
('00100', 'Helsinki'),
('33100', 'Tampere'),
('90100', 'Oulu'),
('96100', 'Rovaniemi'),
('70100', 'Kuopio');

-- Add areas
INSERT INTO `vn`.`alue` (`nimi`) VALUES 
('Pohjois-Suomi'),
('Itä-Suomi'),
('Etelä-Suomi'),
('Länsi-Suomi');

-- Add a few customers
INSERT INTO `vn`.`asiakas` (`postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`) VALUES
('00100', 'Matti', 'Meikäläinen', 'Esimerkkikatu 1', 'matti@example.com', '0401234567'),
('33100', 'Liisa', 'Virtanen', 'Koivukuja 5', 'liisa@example.com', '0509876543'),
('90100', 'Juha', 'Korhonen', 'Mäntytie 10', 'juha@example.com', '0457654321');

-- Add some cottages
INSERT INTO `vn`.`mokki` (`alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`) VALUES
(1, '96100', 'Tunturimökki', 'Tunturitie 1', 120.00, 'Kaunis mökki tunturin laella', 4, 'Sauna, takka, wifi'),
(1, '96100', 'Revontulimökki', 'Revontulentie 5', 150.00, 'Luksus mökki revontulien katseluun', 6, 'Sauna, poreamme, takka, wifi'),
(2, '70100', 'Järvimökki', 'Järventie 10', 100.00, 'Rauhallinen mökki järven rannalla', 4, 'Sauna, vene, kalastuslupa'),
(3, '00100', 'Rantahuvila', 'Rantatie 20', 200.00, 'Moderni huvila meren rannalla', 8, 'Sauna, terassi, grillikatos, wifi');

-- Add services
INSERT INTO `vn`.`palvelu` (`palvelu_id`, `alue_id`, `nimi`, `kuvaus`, `hinta`, `alv`) VALUES
(1, 1, 'Moottorikelkkasafari', '2 tunnin moottorikelkkasafari tunturissa', 80.00, 24.00),
(2, 1, 'Koiravaljakkoajelu', '1 tunnin koiravaljakkoajelu', 100.00, 24.00),
(3, 2, 'Kalastusretki', '4 tunnin opastettu kalastusretki', 60.00, 24.00),
(4, 3, 'SUP-lauta vuokraus', 'SUP-laudan vuokraus päiväksi', 25.00, 24.00);

-- Add some reservations 
INSERT INTO `vn`.`varaus` (`asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`) VALUES
(1, 2, NOW(), NOW(), '2023-06-01 14:00:00', '2023-06-08 12:00:00'),
(2, 3, NOW(), NOW(), '2023-07-15 14:00:00', '2023-07-22 12:00:00'),
(3, 1, NOW(), NOW(), '2023-08-05 14:00:00', '2023-08-12 12:00:00');

-- Add reservation services
INSERT INTO `vn`.`varauksen_palvelut` (`varaus_id`, `palvelu_id`, `lkm`) VALUES
(1, 1, 1),
(1, 2, 2),
(2, 3, 1);

-- Add invoices
INSERT INTO `vn`.`lasku` (`lasku_id`, `varaus_id`, `summa`, `alv`, `maksettu`) VALUES
(1, 1, 1380.00, 331.20, 1),
(2, 2, 760.00, 182.40, 0),
(3, 3, 840.00, 201.60, 0); 