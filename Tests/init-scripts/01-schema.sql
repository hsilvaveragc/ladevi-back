--
-- PostgreSQL database dump
--

\restrict LKDNL8Hi6DyezEWEGMHPTkVyxsw7XzXxfVf5gvSObYRG0yq0leEkHiz8QwnwHL0

-- Dumped from database version 17.6 (Debian 17.6-1.pgdg13+1)
-- Dumped by pg_dump version 17.6 (Debian 17.6-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE IF EXISTS ONLY public."TaxType" DROP CONSTRAINT IF EXISTS "FK_TaxType_Country_CountryId";
ALTER TABLE IF EXISTS ONLY public."State" DROP CONSTRAINT IF EXISTS "FK_State_Country_CountryId";
ALTER TABLE IF EXISTS ONLY public."SoldSpaces" DROP CONSTRAINT IF EXISTS "FK_SoldSpaces_ProductAdvertisingSpaces_ProductAdvertisingSpace~";
ALTER TABLE IF EXISTS ONLY public."SoldSpaces" DROP CONSTRAINT IF EXISTS "FK_SoldSpaces_Contracts_ContractId";
ALTER TABLE IF EXISTS ONLY public."SoldSpaces" DROP CONSTRAINT IF EXISTS "FK_SoldSpaces_AdvertisingSpaceLocationTypes_AdvertisingSpaceLo~";
ALTER TABLE IF EXISTS ONLY public."PublishingOrders" DROP CONSTRAINT IF EXISTS "FK_PublishingOrders_ProductEditions_ProductEditionId";
ALTER TABLE IF EXISTS ONLY public."PublishingOrders" DROP CONSTRAINT IF EXISTS "FK_PublishingOrders_ProductAdvertisingSpaces_ProductAdvertisin~";
ALTER TABLE IF EXISTS ONLY public."PublishingOrders" DROP CONSTRAINT IF EXISTS "FK_PublishingOrders_Contracts_ContractId";
ALTER TABLE IF EXISTS ONLY public."PublishingOrders" DROP CONSTRAINT IF EXISTS "FK_PublishingOrders_Clients_ClientId";
ALTER TABLE IF EXISTS ONLY public."PublishingOrders" DROP CONSTRAINT IF EXISTS "FK_PublishingOrders_ApplicationUsers_SellerId";
ALTER TABLE IF EXISTS ONLY public."PublishingOrders" DROP CONSTRAINT IF EXISTS "FK_PublishingOrders_AdvertisingSpaceLocationTypes_AdvertisingS~";
ALTER TABLE IF EXISTS ONLY public."Products" DROP CONSTRAINT IF EXISTS "FK_Products_ProductTypes_ProductTypeId";
ALTER TABLE IF EXISTS ONLY public."Products" DROP CONSTRAINT IF EXISTS "FK_Products_Country_CountryId";
ALTER TABLE IF EXISTS ONLY public."ProductVolumeDiscount" DROP CONSTRAINT IF EXISTS "FK_ProductVolumeDiscount_Products_ProductId";
ALTER TABLE IF EXISTS ONLY public."ProductLocationDiscount" DROP CONSTRAINT IF EXISTS "FK_ProductLocationDiscount_Products_ProductId";
ALTER TABLE IF EXISTS ONLY public."ProductEditions" DROP CONSTRAINT IF EXISTS "FK_ProductEditions_Products_ProductId";
ALTER TABLE IF EXISTS ONLY public."ProductAdvertisingSpaces" DROP CONSTRAINT IF EXISTS "FK_ProductAdvertisingSpaces_Products_ProductId";
ALTER TABLE IF EXISTS ONLY public."ProductAdvertisingSpaceVolumeDiscount" DROP CONSTRAINT IF EXISTS "FK_ProductAdvertisingSpaceVolumeDiscount_ProductAdvertisingSpa~";
ALTER TABLE IF EXISTS ONLY public."ProductAdvertisingSpaceLocationDiscount" DROP CONSTRAINT IF EXISTS "FK_ProductAdvertisingSpaceLocationDiscount_ProductAdvertisingS~";
ALTER TABLE IF EXISTS ONLY public."District" DROP CONSTRAINT IF EXISTS "FK_District_State_StateId";
ALTER TABLE IF EXISTS ONLY public."CurrencyParities" DROP CONSTRAINT IF EXISTS "FK_CurrencyParities_Currency_CurrencyId";
ALTER TABLE IF EXISTS ONLY public."Contracts" DROP CONSTRAINT IF EXISTS "FK_Contracts_Products_ProductId";
ALTER TABLE IF EXISTS ONLY public."Contracts" DROP CONSTRAINT IF EXISTS "FK_Contracts_PaymentMethods_PaymentMethodId";
ALTER TABLE IF EXISTS ONLY public."Contracts" DROP CONSTRAINT IF EXISTS "FK_Contracts_Currency_CurrencyId";
ALTER TABLE IF EXISTS ONLY public."Contracts" DROP CONSTRAINT IF EXISTS "FK_Contracts_Country_BillingCountryId";
ALTER TABLE IF EXISTS ONLY public."Contracts" DROP CONSTRAINT IF EXISTS "FK_Contracts_Clients_ClientId";
ALTER TABLE IF EXISTS ONLY public."Contracts" DROP CONSTRAINT IF EXISTS "FK_Contracts_BillingConditions_BillingConditionId";
ALTER TABLE IF EXISTS ONLY public."Contracts" DROP CONSTRAINT IF EXISTS "FK_Contracts_ApplicationUsers_SellerId";
ALTER TABLE IF EXISTS ONLY public."ContractHistoricals" DROP CONSTRAINT IF EXISTS "FK_ContractHistoricals_Contracts_ContractId";
ALTER TABLE IF EXISTS ONLY public."Clients" DROP CONSTRAINT IF EXISTS "FK_Clients_TaxType_TaxTypeId";
ALTER TABLE IF EXISTS ONLY public."Clients" DROP CONSTRAINT IF EXISTS "FK_Clients_TaxCategories_TaxCategoryId";
ALTER TABLE IF EXISTS ONLY public."Clients" DROP CONSTRAINT IF EXISTS "FK_Clients_State_StateId";
ALTER TABLE IF EXISTS ONLY public."Clients" DROP CONSTRAINT IF EXISTS "FK_Clients_District_DistrictId";
ALTER TABLE IF EXISTS ONLY public."Clients" DROP CONSTRAINT IF EXISTS "FK_Clients_Country_CountryId";
ALTER TABLE IF EXISTS ONLY public."Clients" DROP CONSTRAINT IF EXISTS "FK_Clients_City_CityId";
ALTER TABLE IF EXISTS ONLY public."Clients" DROP CONSTRAINT IF EXISTS "FK_Clients_ApplicationUsers_ApplicationUserSellerId";
ALTER TABLE IF EXISTS ONLY public."Clients" DROP CONSTRAINT IF EXISTS "FK_Clients_ApplicationUsers_ApplicationUserDebtCollectorId";
ALTER TABLE IF EXISTS ONLY public."ClientTaxes" DROP CONSTRAINT IF EXISTS "FK_ClientTaxes_TaxType_TaxTypeId";
ALTER TABLE IF EXISTS ONLY public."City" DROP CONSTRAINT IF EXISTS "FK_City_District_DistrictId";
ALTER TABLE IF EXISTS ONLY public."CheckPayments" DROP CONSTRAINT IF EXISTS "FK_CheckPayments_Contracts_ContractId";
ALTER TABLE IF EXISTS ONLY public."AspNetUserTokens" DROP CONSTRAINT IF EXISTS "FK_AspNetUserTokens_AspNetUsers_UserId";
ALTER TABLE IF EXISTS ONLY public."AspNetUserRoles" DROP CONSTRAINT IF EXISTS "FK_AspNetUserRoles_AspNetUsers_UserId";
ALTER TABLE IF EXISTS ONLY public."AspNetUserRoles" DROP CONSTRAINT IF EXISTS "FK_AspNetUserRoles_AspNetRoles_RoleId";
ALTER TABLE IF EXISTS ONLY public."AspNetUserLogins" DROP CONSTRAINT IF EXISTS "FK_AspNetUserLogins_AspNetUsers_UserId";
ALTER TABLE IF EXISTS ONLY public."AspNetUserClaims" DROP CONSTRAINT IF EXISTS "FK_AspNetUserClaims_AspNetUsers_UserId";
ALTER TABLE IF EXISTS ONLY public."AspNetRoleClaims" DROP CONSTRAINT IF EXISTS "FK_AspNetRoleClaims_AspNetRoles_RoleId";
ALTER TABLE IF EXISTS ONLY public."ApplicationUsers" DROP CONSTRAINT IF EXISTS "FK_ApplicationUsers_Country_CountryId";
ALTER TABLE IF EXISTS ONLY public."ApplicationUsers" DROP CONSTRAINT IF EXISTS "FK_ApplicationUsers_AspNetUsers_CredentialsUserId";
ALTER TABLE IF EXISTS ONLY public."ApplicationUsers" DROP CONSTRAINT IF EXISTS "FK_ApplicationUsers_ApplicationRole_ApplicationRoleId";
DROP INDEX IF EXISTS public."UserNameIndex";
DROP INDEX IF EXISTS public."RoleNameIndex";
DROP INDEX IF EXISTS public."IX_TaxType_CountryId";
DROP INDEX IF EXISTS public."IX_State_CountryId";
DROP INDEX IF EXISTS public."IX_SoldSpaces_ProductAdvertisingSpaceId";
DROP INDEX IF EXISTS public."IX_SoldSpaces_ContractId";
DROP INDEX IF EXISTS public."IX_SoldSpaces_AdvertisingSpaceLocationTypeId";
DROP INDEX IF EXISTS public."IX_PublishingOrders_SellerId";
DROP INDEX IF EXISTS public."IX_PublishingOrders_ProductEditionId";
DROP INDEX IF EXISTS public."IX_PublishingOrders_ProductAdvertisingSpaceId";
DROP INDEX IF EXISTS public."IX_PublishingOrders_ContractId";
DROP INDEX IF EXISTS public."IX_PublishingOrders_ClientId";
DROP INDEX IF EXISTS public."IX_PublishingOrders_AdvertisingSpaceLocationTypeId";
DROP INDEX IF EXISTS public."IX_Products_ProductTypeId";
DROP INDEX IF EXISTS public."IX_Products_CountryId";
DROP INDEX IF EXISTS public."IX_ProductVolumeDiscount_ProductId";
DROP INDEX IF EXISTS public."IX_ProductLocationDiscount_ProductId";
DROP INDEX IF EXISTS public."IX_ProductEditions_ProductId";
DROP INDEX IF EXISTS public."IX_ProductAdvertisingSpaces_ProductId";
DROP INDEX IF EXISTS public."IX_ProductAdvertisingSpaceVolumeDiscount_ProductAdvertisingSpa~";
DROP INDEX IF EXISTS public."IX_ProductAdvertisingSpaceLocationDiscount_ProductAdvertisingS~";
DROP INDEX IF EXISTS public."IX_District_StateId";
DROP INDEX IF EXISTS public."IX_CurrencyParities_CurrencyId";
DROP INDEX IF EXISTS public."IX_Contracts_SellerId";
DROP INDEX IF EXISTS public."IX_Contracts_ProductId";
DROP INDEX IF EXISTS public."IX_Contracts_PaymentMethodId";
DROP INDEX IF EXISTS public."IX_Contracts_CurrencyId";
DROP INDEX IF EXISTS public."IX_Contracts_ClientId";
DROP INDEX IF EXISTS public."IX_Contracts_BillingCountryId";
DROP INDEX IF EXISTS public."IX_Contracts_BillingConditionId";
DROP INDEX IF EXISTS public."IX_ContractHistoricals_ContractId";
DROP INDEX IF EXISTS public."IX_Clients_TaxTypeId";
DROP INDEX IF EXISTS public."IX_Clients_TaxCategoryId";
DROP INDEX IF EXISTS public."IX_Clients_StateId";
DROP INDEX IF EXISTS public."IX_Clients_DistrictId";
DROP INDEX IF EXISTS public."IX_Clients_CountryId";
DROP INDEX IF EXISTS public."IX_Clients_CityId";
DROP INDEX IF EXISTS public."IX_Clients_ApplicationUserSellerId";
DROP INDEX IF EXISTS public."IX_Clients_ApplicationUserDebtCollectorId";
DROP INDEX IF EXISTS public."IX_ClientTaxes_TaxTypeId";
DROP INDEX IF EXISTS public."IX_City_DistrictId";
DROP INDEX IF EXISTS public."IX_CheckPayments_ContractId";
DROP INDEX IF EXISTS public."IX_AspNetUserRoles_RoleId";
DROP INDEX IF EXISTS public."IX_AspNetUserLogins_UserId";
DROP INDEX IF EXISTS public."IX_AspNetUserClaims_UserId";
DROP INDEX IF EXISTS public."IX_AspNetRoleClaims_RoleId";
DROP INDEX IF EXISTS public."IX_ApplicationUsers_CredentialsUserId";
DROP INDEX IF EXISTS public."IX_ApplicationUsers_CountryId";
DROP INDEX IF EXISTS public."IX_ApplicationUsers_ApplicationRoleId";
DROP INDEX IF EXISTS public."EmailIndex";
ALTER TABLE IF EXISTS ONLY public."__EFMigrationsHistory" DROP CONSTRAINT IF EXISTS "PK___EFMigrationsHistory";
ALTER TABLE IF EXISTS ONLY public."TaxType" DROP CONSTRAINT IF EXISTS "PK_TaxType";
ALTER TABLE IF EXISTS ONLY public."TaxCategories" DROP CONSTRAINT IF EXISTS "PK_TaxCategories";
ALTER TABLE IF EXISTS ONLY public."State" DROP CONSTRAINT IF EXISTS "PK_State";
ALTER TABLE IF EXISTS ONLY public."SoldSpaces" DROP CONSTRAINT IF EXISTS "PK_SoldSpaces";
ALTER TABLE IF EXISTS ONLY public."ReportOPForProductionExports" DROP CONSTRAINT IF EXISTS "PK_ReportOPForProductionExports";
ALTER TABLE IF EXISTS ONLY public."RefreshTokens" DROP CONSTRAINT IF EXISTS "PK_RefreshTokens";
ALTER TABLE IF EXISTS ONLY public."PublishingOrders" DROP CONSTRAINT IF EXISTS "PK_PublishingOrders";
ALTER TABLE IF EXISTS ONLY public."Products" DROP CONSTRAINT IF EXISTS "PK_Products";
ALTER TABLE IF EXISTS ONLY public."ProductVolumeDiscount" DROP CONSTRAINT IF EXISTS "PK_ProductVolumeDiscount";
ALTER TABLE IF EXISTS ONLY public."ProductTypes" DROP CONSTRAINT IF EXISTS "PK_ProductTypes";
ALTER TABLE IF EXISTS ONLY public."ProductLocationDiscount" DROP CONSTRAINT IF EXISTS "PK_ProductLocationDiscount";
ALTER TABLE IF EXISTS ONLY public."ProductEditions" DROP CONSTRAINT IF EXISTS "PK_ProductEditions";
ALTER TABLE IF EXISTS ONLY public."ProductCurrencyParity" DROP CONSTRAINT IF EXISTS "PK_ProductCurrencyParity";
ALTER TABLE IF EXISTS ONLY public."ProductAdvertisingSpaces" DROP CONSTRAINT IF EXISTS "PK_ProductAdvertisingSpaces";
ALTER TABLE IF EXISTS ONLY public."ProductAdvertisingSpaceVolumeDiscount" DROP CONSTRAINT IF EXISTS "PK_ProductAdvertisingSpaceVolumeDiscount";
ALTER TABLE IF EXISTS ONLY public."ProductAdvertisingSpaceLocationDiscount" DROP CONSTRAINT IF EXISTS "PK_ProductAdvertisingSpaceLocationDiscount";
ALTER TABLE IF EXISTS ONLY public."PaymentMethods" DROP CONSTRAINT IF EXISTS "PK_PaymentMethods";
ALTER TABLE IF EXISTS ONLY public."EuroParities" DROP CONSTRAINT IF EXISTS "PK_EuroParities";
ALTER TABLE IF EXISTS ONLY public."District" DROP CONSTRAINT IF EXISTS "PK_District";
ALTER TABLE IF EXISTS ONLY public."CurrencyParities" DROP CONSTRAINT IF EXISTS "PK_CurrencyParities";
ALTER TABLE IF EXISTS ONLY public."Currency" DROP CONSTRAINT IF EXISTS "PK_Currency";
ALTER TABLE IF EXISTS ONLY public."Country" DROP CONSTRAINT IF EXISTS "PK_Country";
ALTER TABLE IF EXISTS ONLY public."Contracts" DROP CONSTRAINT IF EXISTS "PK_Contracts";
ALTER TABLE IF EXISTS ONLY public."ContractHistoricals" DROP CONSTRAINT IF EXISTS "PK_ContractHistoricals";
ALTER TABLE IF EXISTS ONLY public."Clients" DROP CONSTRAINT IF EXISTS "PK_Clients";
ALTER TABLE IF EXISTS ONLY public."ClientTaxes" DROP CONSTRAINT IF EXISTS "PK_ClientTaxes";
ALTER TABLE IF EXISTS ONLY public."City" DROP CONSTRAINT IF EXISTS "PK_City";
ALTER TABLE IF EXISTS ONLY public."CheckPayments" DROP CONSTRAINT IF EXISTS "PK_CheckPayments";
ALTER TABLE IF EXISTS ONLY public."BillingConditions" DROP CONSTRAINT IF EXISTS "PK_BillingConditions";
ALTER TABLE IF EXISTS ONLY public."Auditory" DROP CONSTRAINT IF EXISTS "PK_Auditory";
ALTER TABLE IF EXISTS ONLY public."AspNetUsers" DROP CONSTRAINT IF EXISTS "PK_AspNetUsers";
ALTER TABLE IF EXISTS ONLY public."AspNetUserTokens" DROP CONSTRAINT IF EXISTS "PK_AspNetUserTokens";
ALTER TABLE IF EXISTS ONLY public."AspNetUserRoles" DROP CONSTRAINT IF EXISTS "PK_AspNetUserRoles";
ALTER TABLE IF EXISTS ONLY public."AspNetUserLogins" DROP CONSTRAINT IF EXISTS "PK_AspNetUserLogins";
ALTER TABLE IF EXISTS ONLY public."AspNetUserClaims" DROP CONSTRAINT IF EXISTS "PK_AspNetUserClaims";
ALTER TABLE IF EXISTS ONLY public."AspNetRoles" DROP CONSTRAINT IF EXISTS "PK_AspNetRoles";
ALTER TABLE IF EXISTS ONLY public."AspNetRoleClaims" DROP CONSTRAINT IF EXISTS "PK_AspNetRoleClaims";
ALTER TABLE IF EXISTS ONLY public."ApplicationUsers" DROP CONSTRAINT IF EXISTS "PK_ApplicationUsers";
ALTER TABLE IF EXISTS ONLY public."ApplicationRole" DROP CONSTRAINT IF EXISTS "PK_ApplicationRole";
ALTER TABLE IF EXISTS ONLY public."AdvertisingSpaceLocationTypes" DROP CONSTRAINT IF EXISTS "PK_AdvertisingSpaceLocationTypes";
ALTER TABLE IF EXISTS ONLY public."MigrationLogs" DROP CONSTRAINT IF EXISTS "MigrationLogs_pkey";
ALTER TABLE IF EXISTS public."MigrationLogs" ALTER COLUMN "Id" DROP DEFAULT;
ALTER TABLE IF EXISTS public."ClientTaxes" ALTER COLUMN "Id" DROP DEFAULT;
DROP TABLE IF EXISTS public."__EFMigrationsHistory";
DROP TABLE IF EXISTS public."TaxType";
DROP TABLE IF EXISTS public."TaxCategories";
DROP TABLE IF EXISTS public."State";
DROP TABLE IF EXISTS public."SoldSpaces";
DROP TABLE IF EXISTS public."ReportOPForProductionExports";
DROP TABLE IF EXISTS public."RefreshTokens";
DROP TABLE IF EXISTS public."PublishingOrders";
DROP TABLE IF EXISTS public."Products";
DROP TABLE IF EXISTS public."ProductVolumeDiscount";
DROP TABLE IF EXISTS public."ProductTypes";
DROP TABLE IF EXISTS public."ProductLocationDiscount";
DROP TABLE IF EXISTS public."ProductEditions";
DROP TABLE IF EXISTS public."ProductCurrencyParity";
DROP TABLE IF EXISTS public."ProductAdvertisingSpaces";
DROP TABLE IF EXISTS public."ProductAdvertisingSpaceVolumeDiscount";
DROP TABLE IF EXISTS public."ProductAdvertisingSpaceLocationDiscount";
DROP TABLE IF EXISTS public."PaymentMethods";
DROP SEQUENCE IF EXISTS public."MigrationLogs_Id_seq";
DROP TABLE IF EXISTS public."MigrationLogs";
DROP TABLE IF EXISTS public."EuroParities";
DROP TABLE IF EXISTS public."District";
DROP TABLE IF EXISTS public."CurrencyParities";
DROP TABLE IF EXISTS public."Currency";
DROP TABLE IF EXISTS public."Country";
DROP TABLE IF EXISTS public."Contracts";
DROP TABLE IF EXISTS public."ContractHistoricals";
DROP TABLE IF EXISTS public."Clients";
DROP SEQUENCE IF EXISTS public."ClientTaxes_Id_seq";
DROP TABLE IF EXISTS public."ClientTaxes";
DROP TABLE IF EXISTS public."City";
DROP TABLE IF EXISTS public."CheckPayments";
DROP TABLE IF EXISTS public."BillingConditions";
DROP TABLE IF EXISTS public."Auditory";
DROP TABLE IF EXISTS public."AspNetUsers";
DROP TABLE IF EXISTS public."AspNetUserTokens";
DROP TABLE IF EXISTS public."AspNetUserRoles";
DROP TABLE IF EXISTS public."AspNetUserLogins";
DROP TABLE IF EXISTS public."AspNetUserClaims";
DROP TABLE IF EXISTS public."AspNetRoles";
DROP TABLE IF EXISTS public."AspNetRoleClaims";
DROP TABLE IF EXISTS public."ApplicationUsers";
DROP TABLE IF EXISTS public."ApplicationRole";
DROP TABLE IF EXISTS public."AdvertisingSpaceLocationTypes";
DROP EXTENSION IF EXISTS citext;
--
-- Name: citext; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS citext WITH SCHEMA public;


--
-- Name: EXTENSION citext; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION citext IS 'data type for case-insensitive character strings';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: AdvertisingSpaceLocationTypes; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."AdvertisingSpaceLocationTypes" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."AdvertisingSpaceLocationTypes" OWNER TO sa;

--
-- Name: AdvertisingSpaceLocationTypes_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."AdvertisingSpaceLocationTypes" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AdvertisingSpaceLocationTypes_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ApplicationRole; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ApplicationRole" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "ShouldHaveCommission" boolean NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."ApplicationRole" OWNER TO sa;

--
-- Name: ApplicationRole_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ApplicationRole" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ApplicationRole_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ApplicationUsers; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ApplicationUsers" (
    "Id" bigint NOT NULL,
    "FullName" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Initials" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "CommisionCoeficient" double precision NOT NULL,
    "CountryId" bigint NOT NULL,
    "CredentialsUserId" text DEFAULT ''::text NOT NULL,
    "ApplicationRoleId" bigint DEFAULT 0 NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."ApplicationUsers" OWNER TO sa;

--
-- Name: ApplicationUsers_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ApplicationUsers" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ApplicationUsers_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetRoleClaims" OWNER TO sa;

--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "NormalizedName" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "ConcurrencyStamp" text
);


ALTER TABLE public."AspNetRoles" OWNER TO sa;

--
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetUserClaims" OWNER TO sa;

--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


ALTER TABLE public."AspNetUserLogins" OWNER TO sa;

--
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


ALTER TABLE public."AspNetUserRoles" OWNER TO sa;

--
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."AspNetUserTokens" OWNER TO sa;

--
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."AspNetUsers" (
    "Id" text NOT NULL,
    "UserName" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "NormalizedUserName" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "Email" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "NormalizedEmail" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


ALTER TABLE public."AspNetUsers" OWNER TO sa;

--
-- Name: Auditory; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."Auditory" (
    "Id" bigint NOT NULL,
    "UserId" bigint NOT NULL,
    "User" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Date" timestamp without time zone NOT NULL,
    "Entity" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "AuditMessage" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."Auditory" OWNER TO sa;

--
-- Name: Auditory_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."Auditory" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Auditory_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: BillingConditions; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."BillingConditions" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Deleted" boolean NOT NULL,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."BillingConditions" OWNER TO sa;

--
-- Name: BillingConditions_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."BillingConditions" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."BillingConditions_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: CheckPayments; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."CheckPayments" (
    "Id" bigint NOT NULL,
    "Order" integer NOT NULL,
    "Date" timestamp without time zone NOT NULL,
    "Total" double precision NOT NULL,
    "ContractId" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."CheckPayments" OWNER TO sa;

--
-- Name: CheckPayments_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."CheckPayments" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."CheckPayments_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: City; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."City" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "DistrictId" bigint NOT NULL,
    "CodigoTelefonico" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "XubioCode" text
);


ALTER TABLE public."City" OWNER TO sa;

--
-- Name: City_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."City" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."City_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ClientTaxes; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ClientTaxes" (
    "Id" bigint NOT NULL,
    "Value" text,
    "ClientId" bigint NOT NULL,
    "TaxTypeId" bigint NOT NULL
);


ALTER TABLE public."ClientTaxes" OWNER TO sa;

--
-- Name: ClientTaxes_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

CREATE SEQUENCE public."ClientTaxes_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."ClientTaxes_Id_seq" OWNER TO sa;

--
-- Name: ClientTaxes_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: sa
--

ALTER SEQUENCE public."ClientTaxes_Id_seq" OWNED BY public."ClientTaxes"."Id";


--
-- Name: Clients; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."Clients" (
    "Id" bigint NOT NULL,
    "BrandName" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "LegalName" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "IsEnabled" boolean NOT NULL,
    "Address" public.citext DEFAULT ''::character varying NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "CityId" bigint,
    "PostalCode" public.citext DEFAULT ''::character varying NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "AlternativeEmail" public.citext DEFAULT ''::public.citext COLLATE pg_catalog."es-AR-x-icu",
    "ApplicationUserDebtCollectorId" bigint DEFAULT 0 NOT NULL,
    "ApplicationUserSellerId" bigint DEFAULT 0 NOT NULL,
    "BillingPointOfSale" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "ElectronicBillByMail" boolean DEFAULT false NOT NULL,
    "ElectronicBillByPaper" boolean DEFAULT false NOT NULL,
    "IsAgency" boolean DEFAULT false NOT NULL,
    "IsComtur" boolean DEFAULT false NOT NULL,
    "MainEmail" public.citext DEFAULT ''::text NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "TelephoneAreaCode" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "TelephoneCountryCode" public.citext DEFAULT ''::character varying NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "TelephoneNumber" public.citext DEFAULT ''::character varying NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "TaxTypeId" bigint DEFAULT 0 NOT NULL,
    "IdentificationValue" public.citext DEFAULT ''::text NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "TaxPercentage" double precision DEFAULT 0.0 NOT NULL,
    "Contact" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "CountryId" bigint NOT NULL,
    "DistrictId" bigint,
    "StateId" bigint NOT NULL,
    "XubioId" bigint,
    "TaxCategoryId" bigint,
    "IsBigCompany" boolean
);


ALTER TABLE public."Clients" OWNER TO sa;

--
-- Name: Clients_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."Clients" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Clients_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ContractHistoricals; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ContractHistoricals" (
    "Id" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "Date" timestamp without time zone NOT NULL,
    "User" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Changes" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "ContractId" bigint NOT NULL
);


ALTER TABLE public."ContractHistoricals" OWNER TO sa;

--
-- Name: ContractHistoricals_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ContractHistoricals" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ContractHistoricals_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Contracts; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."Contracts" (
    "Id" bigint NOT NULL,
    "ProductId" bigint NOT NULL,
    "ClientId" bigint NOT NULL,
    "SellerId" bigint NOT NULL,
    "Start" timestamp without time zone NOT NULL,
    "End" timestamp without time zone NOT NULL,
    "ApplyDiscountForCheck" boolean DEFAULT false NOT NULL,
    "ApplyDiscountForLoyalty" boolean DEFAULT false NOT NULL,
    "ApplyDiscountForOtherCountry" boolean DEFAULT false NOT NULL,
    "ApplyDiscountForSameCountry" boolean DEFAULT false NOT NULL,
    "ApplyDiscountForVolume" boolean DEFAULT false NOT NULL,
    "AppyDiscountForAgency" boolean DEFAULT false NOT NULL,
    "BillingConditionId" bigint DEFAULT 0 NOT NULL,
    "BillingCountryId" bigint DEFAULT 0 NOT NULL,
    "CheckQuantity" integer,
    "ContractDate" timestamp without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "CurrencyId" bigint DEFAULT 0,
    "DaysBetweenChecks" integer,
    "DaysToFirstPayment" integer,
    "DiscountForAgency" double precision,
    "DiscountForCheck" double precision,
    "DiscountForLoyalty" double precision,
    "DiscountForOtherCountry" double precision,
    "DiscountForSameCountry" double precision,
    "InvoiceNumber" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Name" public.citext DEFAULT ''::text NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Number" bigint DEFAULT 0 NOT NULL,
    "PaidOut" boolean,
    "PaymentMethodId" bigint DEFAULT 0 NOT NULL,
    "Total" double precision DEFAULT 0.0 NOT NULL,
    "TotalDiscounts" double precision DEFAULT 0.0 NOT NULL,
    "TotalTaxes" double precision DEFAULT 0.0 NOT NULL,
    "Observations" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "DiscountForVolume" double precision,
    "CurrencyParity" double precision,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "IVA" double precision,
    "UseEuro" boolean DEFAULT false NOT NULL,
    "XubioTransactionId" bigint
);


ALTER TABLE public."Contracts" OWNER TO sa;

--
-- Name: Contracts_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."Contracts" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Contracts_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Country; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."Country" (
    "Id" bigint NOT NULL,
    "Name" text NOT NULL,
    "CodigoTelefonico" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "XubioCode" text
);


ALTER TABLE public."Country" OWNER TO sa;

--
-- Name: Country_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."Country" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Country_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Currency; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."Currency" (
    "Id" bigint NOT NULL,
    "Name" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Deleted" boolean,
    "CountryId" bigint DEFAULT 0 NOT NULL,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "UseEuro" boolean DEFAULT false NOT NULL
);


ALTER TABLE public."Currency" OWNER TO sa;

--
-- Name: CurrencyParities; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."CurrencyParities" (
    "Id" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" text,
    "Start" timestamp without time zone NOT NULL,
    "End" timestamp without time zone NOT NULL,
    "LocalCurrencyToDollarExchangeRate" double precision NOT NULL,
    "CurrencyId" bigint NOT NULL
);


ALTER TABLE public."CurrencyParities" OWNER TO sa;

--
-- Name: CurrencyParities_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."CurrencyParities" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."CurrencyParities_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Currency_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."Currency" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Currency_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: District; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."District" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "StateId" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."District" OWNER TO sa;

--
-- Name: District_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."District" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."District_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: EuroParities; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."EuroParities" (
    "Id" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" text,
    "Start" timestamp without time zone NOT NULL,
    "End" timestamp without time zone NOT NULL,
    "EuroToDollarExchangeRate" double precision NOT NULL
);


ALTER TABLE public."EuroParities" OWNER TO sa;

--
-- Name: EuroParities_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."EuroParities" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."EuroParities_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: MigrationLogs; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."MigrationLogs" (
    "Id" integer NOT NULL,
    "MigrationName" character varying(100) NOT NULL,
    "TableName" character varying(100) NOT NULL,
    "RecordId" integer NOT NULL,
    "ColumnName" character varying(100) NOT NULL,
    "OldValue" text,
    "NewValue" text,
    "DateCreated" timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public."MigrationLogs" OWNER TO sa;

--
-- Name: MigrationLogs_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

CREATE SEQUENCE public."MigrationLogs_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."MigrationLogs_Id_seq" OWNER TO sa;

--
-- Name: MigrationLogs_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: sa
--

ALTER SEQUENCE public."MigrationLogs_Id_seq" OWNED BY public."MigrationLogs"."Id";


--
-- Name: PaymentMethods; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."PaymentMethods" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Deleted" boolean NOT NULL,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."PaymentMethods" OWNER TO sa;

--
-- Name: PaymentMethods_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."PaymentMethods" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."PaymentMethods_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ProductAdvertisingSpaceLocationDiscount; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ProductAdvertisingSpaceLocationDiscount" (
    "Id" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" text,
    "Discount" double precision NOT NULL,
    "ProductAdvertisingSpaceId" bigint NOT NULL,
    "AdvertisingSpaceLocationTypeId" bigint NOT NULL
);


ALTER TABLE public."ProductAdvertisingSpaceLocationDiscount" OWNER TO sa;

--
-- Name: ProductAdvertisingSpaceLocationDiscount_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ProductAdvertisingSpaceLocationDiscount" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ProductAdvertisingSpaceLocationDiscount_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ProductAdvertisingSpaceVolumeDiscount; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ProductAdvertisingSpaceVolumeDiscount" (
    "Id" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" text,
    "RangeStart" bigint NOT NULL,
    "RangeEnd" bigint NOT NULL,
    "Discount" double precision NOT NULL,
    "ProductAdvertisingSpaceId" bigint NOT NULL
);


ALTER TABLE public."ProductAdvertisingSpaceVolumeDiscount" OWNER TO sa;

--
-- Name: ProductAdvertisingSpaceVolumeDiscount_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ProductAdvertisingSpaceVolumeDiscount" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ProductAdvertisingSpaceVolumeDiscount_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ProductAdvertisingSpaces; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ProductAdvertisingSpaces" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "ProductId" bigint NOT NULL,
    "DollarPrice" double precision NOT NULL,
    "Height" double precision NOT NULL,
    "Width" double precision NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "Show" boolean DEFAULT true NOT NULL,
    "DiscountForAgency" double precision DEFAULT 0.0 NOT NULL,
    "DiscountForCheck" double precision DEFAULT 0.0 NOT NULL,
    "DiscountForLoyalty" double precision DEFAULT 0.0 NOT NULL,
    "DiscountForOtherCountry" double precision DEFAULT 0.0 NOT NULL,
    "DiscountForSameCountry" double precision DEFAULT 0.0 NOT NULL
);


ALTER TABLE public."ProductAdvertisingSpaces" OWNER TO sa;

--
-- Name: ProductAdvertisingSpaces_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ProductAdvertisingSpaces" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ProductAdvertisingSpaces_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ProductCurrencyParity; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ProductCurrencyParity" (
    "Id" bigint NOT NULL,
    "Start" timestamp without time zone NOT NULL,
    "End" timestamp without time zone NOT NULL,
    "LocalCurrencyToDollarExchangeRate" double precision NOT NULL,
    "ProductId" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."ProductCurrencyParity" OWNER TO sa;

--
-- Name: ProductCurrencyParity_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ProductCurrencyParity" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ProductCurrencyParity_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ProductEditions; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ProductEditions" (
    "Id" bigint NOT NULL,
    "Code" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "ProductId" bigint NOT NULL,
    "Start" timestamp without time zone NOT NULL,
    "End" timestamp without time zone NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "Closed" boolean DEFAULT false NOT NULL
);


ALTER TABLE public."ProductEditions" OWNER TO sa;

--
-- Name: ProductEditions_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ProductEditions" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ProductEditions_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ProductLocationDiscount; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ProductLocationDiscount" (
    "Id" bigint NOT NULL,
    "Discount" double precision NOT NULL,
    "ProductId" bigint NOT NULL,
    "AdvertisingSpaceLocationTypeId" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."ProductLocationDiscount" OWNER TO sa;

--
-- Name: ProductLocationDiscount_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ProductLocationDiscount" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ProductLocationDiscount_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ProductTypes; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ProductTypes" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."ProductTypes" OWNER TO sa;

--
-- Name: ProductTypes_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ProductTypes" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ProductTypes_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ProductVolumeDiscount; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ProductVolumeDiscount" (
    "Id" bigint NOT NULL,
    "RangeStart" bigint NOT NULL,
    "RangeEnd" bigint NOT NULL,
    "Discount" double precision NOT NULL,
    "ProductId" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."ProductVolumeDiscount" OWNER TO sa;

--
-- Name: ProductVolumeDiscount_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ProductVolumeDiscount" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ProductVolumeDiscount_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Products; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."Products" (
    "Id" bigint NOT NULL,
    "ProductTypeId" bigint NOT NULL,
    "CountryId" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "DiscountForCheck" double precision NOT NULL,
    "DiscountForLoyalty" double precision NOT NULL,
    "DiscountForAgency" double precision NOT NULL,
    "DiscountForSameCountry" double precision NOT NULL,
    "DiscountForOtherCountry" double precision NOT NULL,
    "DiscountSpecialBySeller" double precision NOT NULL,
    "DiscountByManager" double precision NOT NULL,
    "MaxAplicableDiscount" double precision NOT NULL,
    "AliquotForSalesCommission" double precision NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "IVA" double precision DEFAULT 0.0 NOT NULL,
    "XubioProductCode" text,
    "ComturXubioProductCode" text
);


ALTER TABLE public."Products" OWNER TO sa;

--
-- Name: Products_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."Products" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Products_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: PublishingOrders; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."PublishingOrders" (
    "Id" bigint NOT NULL,
    "ProductEditionId" bigint NOT NULL,
    "Latent" boolean NOT NULL,
    "ClientId" bigint NOT NULL,
    "ContractId" bigint,
    "AdvertisingSpaceLocationTypeId" bigint NOT NULL,
    "PageNumber" text DEFAULT ''::text NOT NULL,
    "ProductAdvertisingSpaceId" bigint NOT NULL,
    "InvoiceNumber" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "PaidOut" boolean,
    "Quantity" double precision NOT NULL,
    "Observations" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "SellerId" bigint NOT NULL,
    "SoldSpaceId" bigint,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "CreationDate" timestamp without time zone,
    "LastUpdate" timestamp without time zone,
    "XubioDocumentNumber" text,
    "XubioTransactionId" bigint
);


ALTER TABLE public."PublishingOrders" OWNER TO sa;

--
-- Name: PublishingOrders_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."PublishingOrders" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."PublishingOrders_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: RefreshTokens; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."RefreshTokens" (
    "Id" bigint NOT NULL,
    "Username" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Refreshtoken" text DEFAULT ''::text NOT NULL,
    "Revoked" boolean NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."RefreshTokens" OWNER TO sa;

--
-- Name: RefreshTokens_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."RefreshTokens" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."RefreshTokens_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ReportOPForProductionExports; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."ReportOPForProductionExports" (
    "Id" bigint NOT NULL,
    "Date" timestamp without time zone NOT NULL,
    "ProductEditionId" bigint NOT NULL
);


ALTER TABLE public."ReportOPForProductionExports" OWNER TO sa;

--
-- Name: ReportOPForProductionExports_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."ReportOPForProductionExports" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ReportOPForProductionExports_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: SoldSpaces; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."SoldSpaces" (
    "Id" bigint NOT NULL,
    "AdvertisingSpaceLocationTypeId" bigint NOT NULL,
    "ProductAdvertisingSpaceId" bigint NOT NULL,
    "ContractId" bigint NOT NULL,
    "TypeSpecialDiscount" smallint,
    "DescriptionSpecialDiscount" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "SpecialDiscount" double precision NOT NULL,
    "TypeGerentialDiscount" smallint NOT NULL,
    "DescriptionGerentialDiscount" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "GerentialDiscount" double precision NOT NULL,
    "Quantity" double precision NOT NULL,
    "Balance" double precision NOT NULL,
    "Total" double precision NOT NULL,
    "SubTotal" double precision DEFAULT 0.0 NOT NULL,
    "TotalDiscounts" double precision DEFAULT 0.0 NOT NULL,
    "TotalTaxes" double precision DEFAULT 0.0 NOT NULL,
    "LocationDiscount" double precision DEFAULT 0.0 NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "SpacePrice" double precision DEFAULT 0.0 NOT NULL,
    "UnitPriceWithDiscounts" double precision DEFAULT 0.0 NOT NULL,
    "ApplyDiscountForCheck" boolean DEFAULT false NOT NULL,
    "ApplyDiscountForLoyalty" boolean DEFAULT false NOT NULL,
    "ApplyDiscountForOtherCountry" boolean DEFAULT false NOT NULL,
    "ApplyDiscountForSameCountry" boolean DEFAULT false NOT NULL,
    "AppyDiscountForAgency" boolean DEFAULT false NOT NULL,
    "DiscountForAgency" double precision,
    "DiscountForCheck" double precision,
    "DiscountForLoyalty" double precision,
    "DiscountForOtherCountry" double precision,
    "DiscountForSameCountry" double precision,
    "ApplyDiscountForVolume" boolean DEFAULT false NOT NULL,
    "DiscountForVolume" double precision,
    "XubioDocumentNumber" text
);


ALTER TABLE public."SoldSpaces" OWNER TO sa;

--
-- Name: SoldSpaces_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."SoldSpaces" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."SoldSpaces_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: State; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."State" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "CountryId" bigint NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu",
    "XubioCode" text
);


ALTER TABLE public."State" OWNER TO sa;

--
-- Name: State_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."State" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."State_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: TaxCategories; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."TaxCategories" (
    "Id" bigint NOT NULL,
    "Code" text NOT NULL,
    "Name" text NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" text
);


ALTER TABLE public."TaxCategories" OWNER TO sa;

--
-- Name: TaxCategories_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."TaxCategories" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."TaxCategories_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: TaxType; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."TaxType" (
    "Id" bigint NOT NULL,
    "Name" public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "Order" bigint NOT NULL,
    "OptionsInternal" public.citext DEFAULT ''::public.citext NOT NULL COLLATE pg_catalog."es-AR-x-icu",
    "CountryId" bigint NOT NULL,
    "IsIdentificationField" boolean DEFAULT false NOT NULL,
    "Deleted" boolean,
    "DeletedDate" timestamp without time zone,
    "DeletedUser" public.citext COLLATE pg_catalog."es-AR-x-icu"
);


ALTER TABLE public."TaxType" OWNER TO sa;

--
-- Name: TaxType_Id_seq; Type: SEQUENCE; Schema: public; Owner: sa
--

ALTER TABLE public."TaxType" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."TaxType_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: sa
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO sa;

--
-- Name: ClientTaxes Id; Type: DEFAULT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ClientTaxes" ALTER COLUMN "Id" SET DEFAULT nextval('public."ClientTaxes_Id_seq"'::regclass);


--
-- Name: MigrationLogs Id; Type: DEFAULT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."MigrationLogs" ALTER COLUMN "Id" SET DEFAULT nextval('public."MigrationLogs_Id_seq"'::regclass);


--
-- Name: MigrationLogs MigrationLogs_pkey; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."MigrationLogs"
    ADD CONSTRAINT "MigrationLogs_pkey" PRIMARY KEY ("Id");


--
-- Name: AdvertisingSpaceLocationTypes PK_AdvertisingSpaceLocationTypes; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AdvertisingSpaceLocationTypes"
    ADD CONSTRAINT "PK_AdvertisingSpaceLocationTypes" PRIMARY KEY ("Id");


--
-- Name: ApplicationRole PK_ApplicationRole; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ApplicationRole"
    ADD CONSTRAINT "PK_ApplicationRole" PRIMARY KEY ("Id");


--
-- Name: ApplicationUsers PK_ApplicationUsers; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ApplicationUsers"
    ADD CONSTRAINT "PK_ApplicationUsers" PRIMARY KEY ("Id");


--
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- Name: Auditory PK_Auditory; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Auditory"
    ADD CONSTRAINT "PK_Auditory" PRIMARY KEY ("Id");


--
-- Name: BillingConditions PK_BillingConditions; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."BillingConditions"
    ADD CONSTRAINT "PK_BillingConditions" PRIMARY KEY ("Id");


--
-- Name: CheckPayments PK_CheckPayments; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."CheckPayments"
    ADD CONSTRAINT "PK_CheckPayments" PRIMARY KEY ("Id");


--
-- Name: City PK_City; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."City"
    ADD CONSTRAINT "PK_City" PRIMARY KEY ("Id");


--
-- Name: ClientTaxes PK_ClientTaxes; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ClientTaxes"
    ADD CONSTRAINT "PK_ClientTaxes" PRIMARY KEY ("Id");


--
-- Name: Clients PK_Clients; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "PK_Clients" PRIMARY KEY ("Id");


--
-- Name: ContractHistoricals PK_ContractHistoricals; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ContractHistoricals"
    ADD CONSTRAINT "PK_ContractHistoricals" PRIMARY KEY ("Id");


--
-- Name: Contracts PK_Contracts; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Contracts"
    ADD CONSTRAINT "PK_Contracts" PRIMARY KEY ("Id");


--
-- Name: Country PK_Country; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Country"
    ADD CONSTRAINT "PK_Country" PRIMARY KEY ("Id");


--
-- Name: Currency PK_Currency; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Currency"
    ADD CONSTRAINT "PK_Currency" PRIMARY KEY ("Id");


--
-- Name: CurrencyParities PK_CurrencyParities; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."CurrencyParities"
    ADD CONSTRAINT "PK_CurrencyParities" PRIMARY KEY ("Id");


--
-- Name: District PK_District; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."District"
    ADD CONSTRAINT "PK_District" PRIMARY KEY ("Id");


--
-- Name: EuroParities PK_EuroParities; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."EuroParities"
    ADD CONSTRAINT "PK_EuroParities" PRIMARY KEY ("Id");


--
-- Name: PaymentMethods PK_PaymentMethods; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."PaymentMethods"
    ADD CONSTRAINT "PK_PaymentMethods" PRIMARY KEY ("Id");


--
-- Name: ProductAdvertisingSpaceLocationDiscount PK_ProductAdvertisingSpaceLocationDiscount; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductAdvertisingSpaceLocationDiscount"
    ADD CONSTRAINT "PK_ProductAdvertisingSpaceLocationDiscount" PRIMARY KEY ("Id");


--
-- Name: ProductAdvertisingSpaceVolumeDiscount PK_ProductAdvertisingSpaceVolumeDiscount; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductAdvertisingSpaceVolumeDiscount"
    ADD CONSTRAINT "PK_ProductAdvertisingSpaceVolumeDiscount" PRIMARY KEY ("Id");


--
-- Name: ProductAdvertisingSpaces PK_ProductAdvertisingSpaces; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductAdvertisingSpaces"
    ADD CONSTRAINT "PK_ProductAdvertisingSpaces" PRIMARY KEY ("Id");


--
-- Name: ProductCurrencyParity PK_ProductCurrencyParity; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductCurrencyParity"
    ADD CONSTRAINT "PK_ProductCurrencyParity" PRIMARY KEY ("Id");


--
-- Name: ProductEditions PK_ProductEditions; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductEditions"
    ADD CONSTRAINT "PK_ProductEditions" PRIMARY KEY ("Id");


--
-- Name: ProductLocationDiscount PK_ProductLocationDiscount; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductLocationDiscount"
    ADD CONSTRAINT "PK_ProductLocationDiscount" PRIMARY KEY ("Id");


--
-- Name: ProductTypes PK_ProductTypes; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductTypes"
    ADD CONSTRAINT "PK_ProductTypes" PRIMARY KEY ("Id");


--
-- Name: ProductVolumeDiscount PK_ProductVolumeDiscount; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductVolumeDiscount"
    ADD CONSTRAINT "PK_ProductVolumeDiscount" PRIMARY KEY ("Id");


--
-- Name: Products PK_Products; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Products"
    ADD CONSTRAINT "PK_Products" PRIMARY KEY ("Id");


--
-- Name: PublishingOrders PK_PublishingOrders; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."PublishingOrders"
    ADD CONSTRAINT "PK_PublishingOrders" PRIMARY KEY ("Id");


--
-- Name: RefreshTokens PK_RefreshTokens; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id");


--
-- Name: ReportOPForProductionExports PK_ReportOPForProductionExports; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ReportOPForProductionExports"
    ADD CONSTRAINT "PK_ReportOPForProductionExports" PRIMARY KEY ("Id");


--
-- Name: SoldSpaces PK_SoldSpaces; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."SoldSpaces"
    ADD CONSTRAINT "PK_SoldSpaces" PRIMARY KEY ("Id");


--
-- Name: State PK_State; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."State"
    ADD CONSTRAINT "PK_State" PRIMARY KEY ("Id");


--
-- Name: TaxCategories PK_TaxCategories; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."TaxCategories"
    ADD CONSTRAINT "PK_TaxCategories" PRIMARY KEY ("Id");


--
-- Name: TaxType PK_TaxType; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."TaxType"
    ADD CONSTRAINT "PK_TaxType" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "EmailIndex" ON public."AspNetUsers" USING btree ("NormalizedEmail");


--
-- Name: IX_ApplicationUsers_ApplicationRoleId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ApplicationUsers_ApplicationRoleId" ON public."ApplicationUsers" USING btree ("ApplicationRoleId");


--
-- Name: IX_ApplicationUsers_CountryId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ApplicationUsers_CountryId" ON public."ApplicationUsers" USING btree ("CountryId");


--
-- Name: IX_ApplicationUsers_CredentialsUserId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ApplicationUsers_CredentialsUserId" ON public."ApplicationUsers" USING btree ("CredentialsUserId");


--
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON public."AspNetRoleClaims" USING btree ("RoleId");


--
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON public."AspNetUserClaims" USING btree ("UserId");


--
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON public."AspNetUserLogins" USING btree ("UserId");


--
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles" USING btree ("RoleId");


--
-- Name: IX_CheckPayments_ContractId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_CheckPayments_ContractId" ON public."CheckPayments" USING btree ("ContractId");


--
-- Name: IX_City_DistrictId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_City_DistrictId" ON public."City" USING btree ("DistrictId");


--
-- Name: IX_ClientTaxes_TaxTypeId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ClientTaxes_TaxTypeId" ON public."ClientTaxes" USING btree ("TaxTypeId");


--
-- Name: IX_Clients_ApplicationUserDebtCollectorId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Clients_ApplicationUserDebtCollectorId" ON public."Clients" USING btree ("ApplicationUserDebtCollectorId");


--
-- Name: IX_Clients_ApplicationUserSellerId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Clients_ApplicationUserSellerId" ON public."Clients" USING btree ("ApplicationUserSellerId");


--
-- Name: IX_Clients_CityId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Clients_CityId" ON public."Clients" USING btree ("CityId");


--
-- Name: IX_Clients_CountryId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Clients_CountryId" ON public."Clients" USING btree ("CountryId");


--
-- Name: IX_Clients_DistrictId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Clients_DistrictId" ON public."Clients" USING btree ("DistrictId");


--
-- Name: IX_Clients_StateId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Clients_StateId" ON public."Clients" USING btree ("StateId");


--
-- Name: IX_Clients_TaxCategoryId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Clients_TaxCategoryId" ON public."Clients" USING btree ("TaxCategoryId");


--
-- Name: IX_Clients_TaxTypeId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Clients_TaxTypeId" ON public."Clients" USING btree ("TaxTypeId");


--
-- Name: IX_ContractHistoricals_ContractId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ContractHistoricals_ContractId" ON public."ContractHistoricals" USING btree ("ContractId");


--
-- Name: IX_Contracts_BillingConditionId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Contracts_BillingConditionId" ON public."Contracts" USING btree ("BillingConditionId");


--
-- Name: IX_Contracts_BillingCountryId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Contracts_BillingCountryId" ON public."Contracts" USING btree ("BillingCountryId");


--
-- Name: IX_Contracts_ClientId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Contracts_ClientId" ON public."Contracts" USING btree ("ClientId");


--
-- Name: IX_Contracts_CurrencyId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Contracts_CurrencyId" ON public."Contracts" USING btree ("CurrencyId");


--
-- Name: IX_Contracts_PaymentMethodId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Contracts_PaymentMethodId" ON public."Contracts" USING btree ("PaymentMethodId");


--
-- Name: IX_Contracts_ProductId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Contracts_ProductId" ON public."Contracts" USING btree ("ProductId");


--
-- Name: IX_Contracts_SellerId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Contracts_SellerId" ON public."Contracts" USING btree ("SellerId");


--
-- Name: IX_CurrencyParities_CurrencyId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_CurrencyParities_CurrencyId" ON public."CurrencyParities" USING btree ("CurrencyId");


--
-- Name: IX_District_StateId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_District_StateId" ON public."District" USING btree ("StateId");


--
-- Name: IX_ProductAdvertisingSpaceLocationDiscount_ProductAdvertisingS~; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ProductAdvertisingSpaceLocationDiscount_ProductAdvertisingS~" ON public."ProductAdvertisingSpaceLocationDiscount" USING btree ("ProductAdvertisingSpaceId");


--
-- Name: IX_ProductAdvertisingSpaceVolumeDiscount_ProductAdvertisingSpa~; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ProductAdvertisingSpaceVolumeDiscount_ProductAdvertisingSpa~" ON public."ProductAdvertisingSpaceVolumeDiscount" USING btree ("ProductAdvertisingSpaceId");


--
-- Name: IX_ProductAdvertisingSpaces_ProductId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ProductAdvertisingSpaces_ProductId" ON public."ProductAdvertisingSpaces" USING btree ("ProductId");


--
-- Name: IX_ProductEditions_ProductId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ProductEditions_ProductId" ON public."ProductEditions" USING btree ("ProductId");


--
-- Name: IX_ProductLocationDiscount_ProductId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ProductLocationDiscount_ProductId" ON public."ProductLocationDiscount" USING btree ("ProductId");


--
-- Name: IX_ProductVolumeDiscount_ProductId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_ProductVolumeDiscount_ProductId" ON public."ProductVolumeDiscount" USING btree ("ProductId");


--
-- Name: IX_Products_CountryId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Products_CountryId" ON public."Products" USING btree ("CountryId");


--
-- Name: IX_Products_ProductTypeId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_Products_ProductTypeId" ON public."Products" USING btree ("ProductTypeId");


--
-- Name: IX_PublishingOrders_AdvertisingSpaceLocationTypeId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_PublishingOrders_AdvertisingSpaceLocationTypeId" ON public."PublishingOrders" USING btree ("AdvertisingSpaceLocationTypeId");


--
-- Name: IX_PublishingOrders_ClientId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_PublishingOrders_ClientId" ON public."PublishingOrders" USING btree ("ClientId");


--
-- Name: IX_PublishingOrders_ContractId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_PublishingOrders_ContractId" ON public."PublishingOrders" USING btree ("ContractId");


--
-- Name: IX_PublishingOrders_ProductAdvertisingSpaceId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_PublishingOrders_ProductAdvertisingSpaceId" ON public."PublishingOrders" USING btree ("ProductAdvertisingSpaceId");


--
-- Name: IX_PublishingOrders_ProductEditionId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_PublishingOrders_ProductEditionId" ON public."PublishingOrders" USING btree ("ProductEditionId");


--
-- Name: IX_PublishingOrders_SellerId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_PublishingOrders_SellerId" ON public."PublishingOrders" USING btree ("SellerId");


--
-- Name: IX_SoldSpaces_AdvertisingSpaceLocationTypeId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_SoldSpaces_AdvertisingSpaceLocationTypeId" ON public."SoldSpaces" USING btree ("AdvertisingSpaceLocationTypeId");


--
-- Name: IX_SoldSpaces_ContractId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_SoldSpaces_ContractId" ON public."SoldSpaces" USING btree ("ContractId");


--
-- Name: IX_SoldSpaces_ProductAdvertisingSpaceId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_SoldSpaces_ProductAdvertisingSpaceId" ON public."SoldSpaces" USING btree ("ProductAdvertisingSpaceId");


--
-- Name: IX_State_CountryId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_State_CountryId" ON public."State" USING btree ("CountryId");


--
-- Name: IX_TaxType_CountryId; Type: INDEX; Schema: public; Owner: sa
--

CREATE INDEX "IX_TaxType_CountryId" ON public."TaxType" USING btree ("CountryId");


--
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: sa
--

CREATE UNIQUE INDEX "RoleNameIndex" ON public."AspNetRoles" USING btree ("NormalizedName") WHERE ("NormalizedName" IS NOT NULL);


--
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: sa
--

CREATE UNIQUE INDEX "UserNameIndex" ON public."AspNetUsers" USING btree ("NormalizedUserName") WHERE ("NormalizedUserName" IS NOT NULL);


--
-- Name: ApplicationUsers FK_ApplicationUsers_ApplicationRole_ApplicationRoleId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ApplicationUsers"
    ADD CONSTRAINT "FK_ApplicationUsers_ApplicationRole_ApplicationRoleId" FOREIGN KEY ("ApplicationRoleId") REFERENCES public."ApplicationRole"("Id") ON DELETE RESTRICT;


--
-- Name: ApplicationUsers FK_ApplicationUsers_AspNetUsers_CredentialsUserId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ApplicationUsers"
    ADD CONSTRAINT "FK_ApplicationUsers_AspNetUsers_CredentialsUserId" FOREIGN KEY ("CredentialsUserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: ApplicationUsers FK_ApplicationUsers_Country_CountryId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ApplicationUsers"
    ADD CONSTRAINT "FK_ApplicationUsers_Country_CountryId" FOREIGN KEY ("CountryId") REFERENCES public."Country"("Id") ON DELETE RESTRICT;


--
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: CheckPayments FK_CheckPayments_Contracts_ContractId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."CheckPayments"
    ADD CONSTRAINT "FK_CheckPayments_Contracts_ContractId" FOREIGN KEY ("ContractId") REFERENCES public."Contracts"("Id") ON DELETE CASCADE;


--
-- Name: City FK_City_District_DistrictId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."City"
    ADD CONSTRAINT "FK_City_District_DistrictId" FOREIGN KEY ("DistrictId") REFERENCES public."District"("Id") ON DELETE CASCADE;


--
-- Name: ClientTaxes FK_ClientTaxes_TaxType_TaxTypeId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ClientTaxes"
    ADD CONSTRAINT "FK_ClientTaxes_TaxType_TaxTypeId" FOREIGN KEY ("TaxTypeId") REFERENCES public."TaxType"("Id") ON DELETE RESTRICT;


--
-- Name: Clients FK_Clients_ApplicationUsers_ApplicationUserDebtCollectorId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "FK_Clients_ApplicationUsers_ApplicationUserDebtCollectorId" FOREIGN KEY ("ApplicationUserDebtCollectorId") REFERENCES public."ApplicationUsers"("Id") ON DELETE RESTRICT;


--
-- Name: Clients FK_Clients_ApplicationUsers_ApplicationUserSellerId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "FK_Clients_ApplicationUsers_ApplicationUserSellerId" FOREIGN KEY ("ApplicationUserSellerId") REFERENCES public."ApplicationUsers"("Id") ON DELETE RESTRICT;


--
-- Name: Clients FK_Clients_City_CityId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "FK_Clients_City_CityId" FOREIGN KEY ("CityId") REFERENCES public."City"("Id") ON DELETE RESTRICT;


--
-- Name: Clients FK_Clients_Country_CountryId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "FK_Clients_Country_CountryId" FOREIGN KEY ("CountryId") REFERENCES public."Country"("Id") ON DELETE CASCADE;


--
-- Name: Clients FK_Clients_District_DistrictId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "FK_Clients_District_DistrictId" FOREIGN KEY ("DistrictId") REFERENCES public."District"("Id") ON DELETE RESTRICT;


--
-- Name: Clients FK_Clients_State_StateId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "FK_Clients_State_StateId" FOREIGN KEY ("StateId") REFERENCES public."State"("Id") ON DELETE CASCADE;


--
-- Name: Clients FK_Clients_TaxCategories_TaxCategoryId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "FK_Clients_TaxCategories_TaxCategoryId" FOREIGN KEY ("TaxCategoryId") REFERENCES public."TaxCategories"("Id");


--
-- Name: Clients FK_Clients_TaxType_TaxTypeId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "FK_Clients_TaxType_TaxTypeId" FOREIGN KEY ("TaxTypeId") REFERENCES public."TaxType"("Id") ON DELETE CASCADE;


--
-- Name: ContractHistoricals FK_ContractHistoricals_Contracts_ContractId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ContractHistoricals"
    ADD CONSTRAINT "FK_ContractHistoricals_Contracts_ContractId" FOREIGN KEY ("ContractId") REFERENCES public."Contracts"("Id") ON DELETE CASCADE;


--
-- Name: Contracts FK_Contracts_ApplicationUsers_SellerId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Contracts"
    ADD CONSTRAINT "FK_Contracts_ApplicationUsers_SellerId" FOREIGN KEY ("SellerId") REFERENCES public."ApplicationUsers"("Id") ON DELETE RESTRICT;


--
-- Name: Contracts FK_Contracts_BillingConditions_BillingConditionId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Contracts"
    ADD CONSTRAINT "FK_Contracts_BillingConditions_BillingConditionId" FOREIGN KEY ("BillingConditionId") REFERENCES public."BillingConditions"("Id") ON DELETE CASCADE;


--
-- Name: Contracts FK_Contracts_Clients_ClientId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Contracts"
    ADD CONSTRAINT "FK_Contracts_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES public."Clients"("Id") ON DELETE RESTRICT;


--
-- Name: Contracts FK_Contracts_Country_BillingCountryId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Contracts"
    ADD CONSTRAINT "FK_Contracts_Country_BillingCountryId" FOREIGN KEY ("BillingCountryId") REFERENCES public."Country"("Id") ON DELETE CASCADE;


--
-- Name: Contracts FK_Contracts_Currency_CurrencyId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Contracts"
    ADD CONSTRAINT "FK_Contracts_Currency_CurrencyId" FOREIGN KEY ("CurrencyId") REFERENCES public."Currency"("Id");


--
-- Name: Contracts FK_Contracts_PaymentMethods_PaymentMethodId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Contracts"
    ADD CONSTRAINT "FK_Contracts_PaymentMethods_PaymentMethodId" FOREIGN KEY ("PaymentMethodId") REFERENCES public."PaymentMethods"("Id") ON DELETE CASCADE;


--
-- Name: Contracts FK_Contracts_Products_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Contracts"
    ADD CONSTRAINT "FK_Contracts_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES public."Products"("Id") ON DELETE RESTRICT;


--
-- Name: CurrencyParities FK_CurrencyParities_Currency_CurrencyId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."CurrencyParities"
    ADD CONSTRAINT "FK_CurrencyParities_Currency_CurrencyId" FOREIGN KEY ("CurrencyId") REFERENCES public."Currency"("Id") ON DELETE RESTRICT;


--
-- Name: District FK_District_State_StateId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."District"
    ADD CONSTRAINT "FK_District_State_StateId" FOREIGN KEY ("StateId") REFERENCES public."State"("Id") ON DELETE CASCADE;


--
-- Name: ProductAdvertisingSpaceLocationDiscount FK_ProductAdvertisingSpaceLocationDiscount_ProductAdvertisingS~; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductAdvertisingSpaceLocationDiscount"
    ADD CONSTRAINT "FK_ProductAdvertisingSpaceLocationDiscount_ProductAdvertisingS~" FOREIGN KEY ("ProductAdvertisingSpaceId") REFERENCES public."ProductAdvertisingSpaces"("Id") ON DELETE CASCADE;


--
-- Name: ProductAdvertisingSpaceVolumeDiscount FK_ProductAdvertisingSpaceVolumeDiscount_ProductAdvertisingSpa~; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductAdvertisingSpaceVolumeDiscount"
    ADD CONSTRAINT "FK_ProductAdvertisingSpaceVolumeDiscount_ProductAdvertisingSpa~" FOREIGN KEY ("ProductAdvertisingSpaceId") REFERENCES public."ProductAdvertisingSpaces"("Id") ON DELETE CASCADE;


--
-- Name: ProductAdvertisingSpaces FK_ProductAdvertisingSpaces_Products_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductAdvertisingSpaces"
    ADD CONSTRAINT "FK_ProductAdvertisingSpaces_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES public."Products"("Id") ON DELETE CASCADE;


--
-- Name: ProductEditions FK_ProductEditions_Products_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductEditions"
    ADD CONSTRAINT "FK_ProductEditions_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES public."Products"("Id") ON DELETE CASCADE;


--
-- Name: ProductLocationDiscount FK_ProductLocationDiscount_Products_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductLocationDiscount"
    ADD CONSTRAINT "FK_ProductLocationDiscount_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES public."Products"("Id") ON DELETE CASCADE;


--
-- Name: ProductVolumeDiscount FK_ProductVolumeDiscount_Products_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."ProductVolumeDiscount"
    ADD CONSTRAINT "FK_ProductVolumeDiscount_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES public."Products"("Id") ON DELETE CASCADE;


--
-- Name: Products FK_Products_Country_CountryId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Products"
    ADD CONSTRAINT "FK_Products_Country_CountryId" FOREIGN KEY ("CountryId") REFERENCES public."Country"("Id") ON DELETE CASCADE;


--
-- Name: Products FK_Products_ProductTypes_ProductTypeId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."Products"
    ADD CONSTRAINT "FK_Products_ProductTypes_ProductTypeId" FOREIGN KEY ("ProductTypeId") REFERENCES public."ProductTypes"("Id") ON DELETE CASCADE;


--
-- Name: PublishingOrders FK_PublishingOrders_AdvertisingSpaceLocationTypes_AdvertisingS~; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."PublishingOrders"
    ADD CONSTRAINT "FK_PublishingOrders_AdvertisingSpaceLocationTypes_AdvertisingS~" FOREIGN KEY ("AdvertisingSpaceLocationTypeId") REFERENCES public."AdvertisingSpaceLocationTypes"("Id") ON DELETE CASCADE;


--
-- Name: PublishingOrders FK_PublishingOrders_ApplicationUsers_SellerId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."PublishingOrders"
    ADD CONSTRAINT "FK_PublishingOrders_ApplicationUsers_SellerId" FOREIGN KEY ("SellerId") REFERENCES public."ApplicationUsers"("Id") ON DELETE CASCADE;


--
-- Name: PublishingOrders FK_PublishingOrders_Clients_ClientId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."PublishingOrders"
    ADD CONSTRAINT "FK_PublishingOrders_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES public."Clients"("Id") ON DELETE CASCADE;


--
-- Name: PublishingOrders FK_PublishingOrders_Contracts_ContractId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."PublishingOrders"
    ADD CONSTRAINT "FK_PublishingOrders_Contracts_ContractId" FOREIGN KEY ("ContractId") REFERENCES public."Contracts"("Id") ON DELETE RESTRICT;


--
-- Name: PublishingOrders FK_PublishingOrders_ProductAdvertisingSpaces_ProductAdvertisin~; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."PublishingOrders"
    ADD CONSTRAINT "FK_PublishingOrders_ProductAdvertisingSpaces_ProductAdvertisin~" FOREIGN KEY ("ProductAdvertisingSpaceId") REFERENCES public."ProductAdvertisingSpaces"("Id") ON DELETE CASCADE;


--
-- Name: PublishingOrders FK_PublishingOrders_ProductEditions_ProductEditionId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."PublishingOrders"
    ADD CONSTRAINT "FK_PublishingOrders_ProductEditions_ProductEditionId" FOREIGN KEY ("ProductEditionId") REFERENCES public."ProductEditions"("Id") ON DELETE CASCADE;


--
-- Name: SoldSpaces FK_SoldSpaces_AdvertisingSpaceLocationTypes_AdvertisingSpaceLo~; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."SoldSpaces"
    ADD CONSTRAINT "FK_SoldSpaces_AdvertisingSpaceLocationTypes_AdvertisingSpaceLo~" FOREIGN KEY ("AdvertisingSpaceLocationTypeId") REFERENCES public."AdvertisingSpaceLocationTypes"("Id") ON DELETE CASCADE;


--
-- Name: SoldSpaces FK_SoldSpaces_Contracts_ContractId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."SoldSpaces"
    ADD CONSTRAINT "FK_SoldSpaces_Contracts_ContractId" FOREIGN KEY ("ContractId") REFERENCES public."Contracts"("Id") ON DELETE CASCADE;


--
-- Name: SoldSpaces FK_SoldSpaces_ProductAdvertisingSpaces_ProductAdvertisingSpace~; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."SoldSpaces"
    ADD CONSTRAINT "FK_SoldSpaces_ProductAdvertisingSpaces_ProductAdvertisingSpace~" FOREIGN KEY ("ProductAdvertisingSpaceId") REFERENCES public."ProductAdvertisingSpaces"("Id") ON DELETE CASCADE;


--
-- Name: State FK_State_Country_CountryId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."State"
    ADD CONSTRAINT "FK_State_Country_CountryId" FOREIGN KEY ("CountryId") REFERENCES public."Country"("Id") ON DELETE CASCADE;


--
-- Name: TaxType FK_TaxType_Country_CountryId; Type: FK CONSTRAINT; Schema: public; Owner: sa
--

ALTER TABLE ONLY public."TaxType"
    ADD CONSTRAINT "FK_TaxType_Country_CountryId" FOREIGN KEY ("CountryId") REFERENCES public."Country"("Id") ON DELETE CASCADE;


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: pg_database_owner
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

\unrestrict LKDNL8Hi6DyezEWEGMHPTkVyxsw7XzXxfVf5gvSObYRG0yq0leEkHiz8QwnwHL0

