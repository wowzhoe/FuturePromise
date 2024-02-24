using UnityEngine;

namespace SmartTutor.External.Future
{
    public class Form
    {
        public WWWForm form;

        public Form()
        {
            this.form = new WWWForm();
        }

        public WWWForm MakeForm(string nameText, string text, string nameBinary, byte[] dataBinary)
        {
            AddField(nameText, text);
            AddBinaryField(nameBinary, dataBinary);
            return new WWWForm();
        }

        public void AddBinaryField(string name, byte[] data)
        {
            this.form.AddBinaryData(name, data);
        }

        public void AddField(string name, string data)
        {
            this.form.AddField(name, data);
        }
    }
}
