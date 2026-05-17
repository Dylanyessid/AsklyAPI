namespace AcaHelpAPI.DTOs
{
    public record GetTagResponseDTO
    {
        public required int id { get; set; }
        public required string name { get; set; }
    }
    public record  GetMultipleTagsResponseDTO
    {
        public List<GetTagResponseDTO> tags { get; set; }
   


    }
}
