using LearningApp.Models;

namespace LearningApp.Selectors
{
    public class CourseItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SectionHeaderTemplate { get; set; }
        public DataTemplate SubSectionTemplate { get; set; }
        public DataTemplate VideoTemplate { get; set; }
        public DataTemplate AssessmentTemplate { get; set; }
        public DataTemplate EmptyNoteTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                SectionHeaderItem => SectionHeaderTemplate,
                SubSectionItem => SubSectionTemplate,
                VideoItem => VideoTemplate,
                AssessmentItem => AssessmentTemplate,
                EmptyNoteItem => EmptyNoteTemplate,
                _ => SectionHeaderTemplate
            };
        }
    }
}