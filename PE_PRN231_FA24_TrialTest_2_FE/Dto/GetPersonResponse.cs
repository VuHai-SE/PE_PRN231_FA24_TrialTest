namespace PE_PRN231_FA24_TrialTest_2_FE.Dto
{
    public class GetPersonResponse
    {
        public int PersonId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDay { get; set; }
        public string Phone { get; set; }
        public List<VirusInfo> Viruses { get; set; }
    }
}
