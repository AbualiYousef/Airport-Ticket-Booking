namespace DataAccess;

public static class ModelsCsvHeaders
{
    public const string FlightCsvHeader =
        "id,departure_country,destination_country,departure_date,departure_airport,destination_airport,classes,prices,capacities";

    public const string BookingCsvHeader =
        "id,flight_id,passenger_id,flight_class,booking_date";

    public const string PassengerCsvHeader =
        "id,name";
}//End of ModelsCsvHeaders class