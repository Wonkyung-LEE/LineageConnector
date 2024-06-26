﻿using Microsoft.Win32;
using System.ServiceProcess;
using System.Windows.Forms;

namespace LineageConnector
{
    partial class ConnectorMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectorMain));
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.progressBar_download = new System.Windows.Forms.ProgressBar();
            this.label_Status = new System.Windows.Forms.Label();
            this.label_Downloaded = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(30, 265);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 59);
            this.button1.TabIndex = 0;
            this.button1.Text = "서버 접속";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(30, 72);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(700, 25);
            this.textBox1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(760, 72);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(132, 27);
            this.button2.TabIndex = 2;
            this.button2.Text = "린빈검색";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(380, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "린빈검색 버튼을 클릭하여 리니지 실행파일을 선택하여 주세요.";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(188, 265);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(140, 59);
            this.button3.TabIndex = 4;
            this.button3.Text = "프로그램 닫기";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // progressBar_download
            // 
            this.progressBar_download.Location = new System.Drawing.Point(30, 121);
            this.progressBar_download.Name = "progressBar_download";
            this.progressBar_download.Size = new System.Drawing.Size(421, 37);
            this.progressBar_download.TabIndex = 5;
            // 
            // label_Status
            // 
            this.label_Status.AutoSize = true;
            this.label_Status.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_Status.Location = new System.Drawing.Point(26, 170);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(164, 20);
            this.label_Status.TabIndex = 6;
            this.label_Status.Text = "접속기 시작 중...";
            // 
            // label_Downloaded
            // 
            this.label_Downloaded.AutoSize = true;
            this.label_Downloaded.Location = new System.Drawing.Point(586, 131);
            this.label_Downloaded.Name = "label_Downloaded";
            this.label_Downloaded.Size = new System.Drawing.Size(17, 18);
            this.label_Downloaded.TabIndex = 7;
            this.label_Downloaded.Text = "/";
            // 
            // ConnectorMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(938, 591);
            this.Controls.Add(this.label_Downloaded);
            this.Controls.Add(this.label_Status);
            this.Controls.Add(this.progressBar_download);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConnectorMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Connector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private OpenFileDialog openFileDialog1;
        private TextBox textBox1;
        private Button button2;
        private Label label1;
        private Button button3;
        private ProgressBar progressBar_download;
        private Label label_Status;
        private Label label_Downloaded;
    }
}

